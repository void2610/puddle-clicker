using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace PuddleClicker.View
{
    public class RippleController : MonoBehaviour
    {
        [SerializeField] private Renderer targetRenderer;

        [Header("プレーンサイズ設定")]
        [SerializeField] private float planeWidth = 10f;
        [SerializeField] private float planeHeight = 10f;

        private Material _material;
        private int _currentRippleIndex;
        private CancellationTokenSource _cts;

        // シェーダープロパティID
        private static readonly int[] _rippleOriginIds =
        {
            Shader.PropertyToID("_Ripple1Origin"),
            Shader.PropertyToID("_Ripple2Origin"),
            Shader.PropertyToID("_Ripple3Origin"),
            Shader.PropertyToID("_Ripple4Origin")
        };

        private static readonly int[] _rippleParamsIds =
        {
            Shader.PropertyToID("_Ripple1Params"),
            Shader.PropertyToID("_Ripple2Params"),
            Shader.PropertyToID("_Ripple3Params"),
            Shader.PropertyToID("_Ripple4Params")
        };

        private const int MAX_RIPPLES = 4;

        private void Awake()
        {
            _material = targetRenderer.material;
            _cts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            if (_material) Destroy(_material);
        }

        public void CreateRipple(Vector3 worldPosition, float scale, float duration, int count = 1)
        {
            var uvPosition = WorldToUV(worldPosition);
            CreateRippleAsync(uvPosition, scale, duration, count, _cts.Token).Forget();
        }

        private async UniTaskVoid CreateRippleAsync(Vector2 uvPosition, float scale, float duration, int count, CancellationToken ct)
        {
            for (var i = 0; i < count; i++)
            {
                if (ct.IsCancellationRequested) return;

                var rippleIndex = _currentRippleIndex;
                _currentRippleIndex = (_currentRippleIndex + 1) % MAX_RIPPLES;

                // 波紋の原点を設定
                _material.SetVector(_rippleOriginIds[rippleIndex], new Vector4(uvPosition.x, uvPosition.y, 0, 0));

                // プログレスをアニメーション
                await LMotion.Create(0f, 1f, duration)
                    .WithEase(Ease.OutQuad)
                    .Bind(progress =>
                    {
                        _material.SetVector(_rippleParamsIds[rippleIndex], new Vector4(progress, scale, 0, 0));
                    })
                    .ToUniTask(ct);

                // 波紋をリセット
                _material.SetVector(_rippleParamsIds[rippleIndex], Vector4.zero);

                // 複数回の場合は少し間隔を空ける
                if (i < count - 1)
                    await UniTask.Delay(100, cancellationToken: ct);
            }
        }

        private Vector2 WorldToUV(Vector3 worldPosition)
        {
            var localPos = transform.InverseTransformPoint(worldPosition);

            var halfWidth = planeWidth / 2f;
            var halfHeight = planeHeight / 2f;

            var u = 1f - (localPos.x + halfWidth) / planeWidth;
            var v = 1f - (localPos.z + halfHeight) / planeHeight;

            return new Vector2(Mathf.Clamp01(u), Mathf.Clamp01(v));
        }
    }
}
