using R3;
using UnityEngine;

namespace PuddleClicker.View
{
    public class PuddleView : MonoBehaviour
    {
        [SerializeField] private Camera renderCamera;
        [SerializeField] private Collider targetCollider;

        public Observable<Vector3> OnClicked => _onClicked;

        private readonly Subject<Vector3> _onClicked = new();

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                var viewportPoint = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0f);
                var ray = renderCamera.ViewportPointToRay(viewportPoint);
                if (Physics.Raycast(ray, out var hit) && hit.collider == targetCollider)
                    _onClicked.OnNext(hit.point);
            }
        }

        private void OnDestroy() => _onClicked.Dispose();
    }
}
