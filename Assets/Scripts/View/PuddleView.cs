using R3;
using UnityEngine;

namespace PuddleClicker.View
{
    public class PuddleView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem rippleEffectPrefab;

        public Observable<Vector3> OnClicked => _onClicked;

        private readonly Subject<Vector3> _onClicked = new();

        public void PlayRippleEffect(Vector3 position)
        {
            var instance = Instantiate(rippleEffectPrefab, position, Quaternion.identity);
            instance.Play();

            // 再生完了後に自動削除
            var duration = instance.main.duration + instance.main.startLifetime.constantMax;
            Destroy(instance.gameObject, duration);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == gameObject)
                {
                    _onClicked.OnNext(hit.point);
                }
            }
        }

        private void OnDestroy() => _onClicked.Dispose();
    }
}
