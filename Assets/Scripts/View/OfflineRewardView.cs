using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PuddleClicker.View
{
    public class OfflineRewardView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Button closeButton;

        public Observable<Unit> OnClosed => _onClosed;

        private readonly Subject<Unit> _onClosed = new();

        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
            panel.SetActive(false);
        }

        public void Show(long reward)
        {
            rewardText.text = $"+{reward:N0} しずく";
            panel.SetActive(true);
        }

        private void Close()
        {
            panel.SetActive(false);
            _onClosed.OnNext(Unit.Default);
        }

        private void OnDestroy() => _onClosed.Dispose();
    }
}
