using R3;
using UnityEngine;

namespace PuddleClicker.View
{
    public class PuddleView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem rippleEffectPrefab;
        [SerializeField] private Camera renderCamera;
        [SerializeField] private Collider targetCollider;

        public Observable<Vector3> OnClicked => _onClicked;

        private readonly Subject<Vector3> _onClicked = new();

        public void PlayRippleEffect(Vector3 position)
        {
            position += Vector3.up * 0.1f; // 少し浮かせる
            var instance = Instantiate(rippleEffectPrefab, position, Quaternion.Euler(-90f, 0f, 0f));
            instance.Play();
            // 再生完了後に自動削除
            var duration = instance.main.duration + instance.main.startLifetime.constantMax;
            Destroy(instance.gameObject, duration);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // スクリーン座標をビューポート座標（0-1）に変換
                var viewportPoint = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0f);

                var ray = renderCamera.ViewportPointToRay(viewportPoint);
                if (Physics.Raycast(ray, out var hit) && hit.collider == targetCollider)
                    _onClicked.OnNext(hit.point);
            }
        }

        private void OnDestroy() => _onClicked.Dispose();
    }
}
