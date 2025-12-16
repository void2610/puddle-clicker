using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using PuddleClicker.Model;
using UnityEngine;

namespace PuddleClicker.View
{
    public class CompanionView : MonoBehaviour
    {
        [SerializeField] private Transform spawnArea;
        [SerializeField] private RippleController rippleController;

        [Header("アニメーション設定")]
        [SerializeField] private float slideRadius = 3f;
        [SerializeField] private float fallHeight = 5f;
        [SerializeField] private float jumpHeight = 1f;

        private readonly List<CompanionInstance> _activeCompanions = new();
        private readonly CancellationTokenSource _cts = new();

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            foreach (var companion in _activeCompanions)
                if (companion.Instance) Destroy(companion.Instance);
        }

        public void SpawnCompanion(CompanionData data)
        {
            var position = GetRandomSpawnPosition();
            var instance = Instantiate(data.Prefab, position, Quaternion.identity, spawnArea);
            var companionInstance = new CompanionInstance(instance, data);
            _activeCompanions.Add(companionInstance);

            // アニメーション開始
            StartAnimation(companionInstance, _cts.Token).Forget();
        }

        private Vector3 GetRandomSpawnPosition()
        {
            var center = spawnArea.position;
            var offset = new Vector3(
                Random.Range(-slideRadius, slideRadius),
                0f,
                Random.Range(-slideRadius, slideRadius)
            );
            return center + offset;
        }

        private async UniTaskVoid StartAnimation(CompanionInstance companion, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && companion.Instance)
            {
                switch (companion.Data.AnimationType)
                {
                    case CompanionAnimationType.Slide:
                        await AnimateSlide(companion, ct);
                        break;
                    case CompanionAnimationType.Fall:
                        await AnimateFall(companion, ct);
                        break;
                    case CompanionAnimationType.Drop:
                        await AnimateDrop(companion, ct);
                        break;
                    case CompanionAnimationType.Jump:
                        await AnimateJump(companion, ct);
                        break;
                }

                // 波紋発生
                if (companion.Data.CreatesRipple)
                    rippleController.CreateRipple(companion.Instance.transform.position, 0.8f, 0.6f);
            }
        }

        private async UniTask AnimateSlide(CompanionInstance companion, CancellationToken ct)
        {
            // 水面を滑るように移動
            var startPos = companion.Instance.transform.position;
            var targetPos = GetRandomSpawnPosition();
            var duration = 2f / companion.Data.AnimationSpeed;

            await LMotion.Create(0f, 1f, duration)
                .WithEase(Ease.InOutSine)
                .Bind(t => companion.Instance.transform.position = Vector3.Lerp(startPos, targetPos, t))
                .ToUniTask(ct);

            await UniTask.Delay(500, cancellationToken: ct);
        }

        private async UniTask AnimateFall(CompanionInstance companion, CancellationToken ct)
        {
            // 上から落ちてくる
            var basePos = GetRandomSpawnPosition();
            var startPos = basePos + Vector3.up * fallHeight;
            var endPos = basePos;
            var duration = 3f / companion.Data.AnimationSpeed;

            companion.Instance.transform.position = startPos;

            await LMotion.Create(0f, 1f, duration)
                .WithEase(Ease.Linear)
                .Bind(t => companion.Instance.transform.position = Vector3.Lerp(startPos, endPos, t))
                .ToUniTask(ct);

            await UniTask.Delay(1000, cancellationToken: ct);
        }

        private async UniTask AnimateDrop(CompanionInstance companion, CancellationToken ct)
        {
            // 雫が落下
            var basePos = GetRandomSpawnPosition();
            var startPos = basePos + Vector3.up * fallHeight;
            var endPos = basePos;
            var duration = 1f / companion.Data.AnimationSpeed;

            companion.Instance.transform.position = startPos;

            await LMotion.Create(0f, 1f, duration)
                .WithEase(Ease.InQuad)
                .Bind(t => companion.Instance.transform.position = Vector3.Lerp(startPos, endPos, t))
                .ToUniTask(ct);

            await UniTask.Delay(2000, cancellationToken: ct);
        }

        private async UniTask AnimateJump(CompanionInstance companion, CancellationToken ct)
        {
            // 定期的にジャンプ
            var basePos = companion.Instance.transform.position;
            var duration = 0.5f / companion.Data.AnimationSpeed;

            // 上昇
            await LMotion.Create(0f, jumpHeight, duration)
                .WithEase(Ease.OutQuad)
                .Bind(y => companion.Instance.transform.position = basePos + Vector3.up * y)
                .ToUniTask(ct);

            // 下降
            await LMotion.Create(jumpHeight, 0f, duration)
                .WithEase(Ease.InQuad)
                .Bind(y => companion.Instance.transform.position = basePos + Vector3.up * y)
                .ToUniTask(ct);

            await UniTask.Delay(1500, cancellationToken: ct);
        }

        private class CompanionInstance
        {
            public GameObject Instance { get; }
            public CompanionData Data { get; }

            public CompanionInstance(GameObject instance, CompanionData data)
            {
                Instance = instance;
                Data = data;
            }
        }
    }
}
