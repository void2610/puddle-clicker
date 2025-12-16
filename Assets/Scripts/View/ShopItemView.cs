using R3;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PuddleClicker.View
{
    public class ShopItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Button actionButton;
        [SerializeField] private TextMeshProUGUI buttonText;

        public Observable<Unit> OnActionClicked => actionButton.OnClickAsObservable();
        public int ItemIndex => _itemIndex;

        private int _itemIndex;

        public void Initialize(int index, string name, long price, string buttonLabel)
        {
            _itemIndex = index;
            nameText.text = name;
            UpdatePrice(price);
            buttonText.text = buttonLabel;

            if (countText)
                countText.gameObject.SetActive(false);
        }

        public void UpdatePrice(long price) => priceText.text = price > 0 ? $"{price:N0}" : "所持";

        public void UpdateCount(int count)
        {
            if (!countText) return;
            countText.gameObject.SetActive(true);
            countText.text = $"×{count}";
        }

        public void SetButtonInteractable(bool interactable) => actionButton.interactable = interactable;

        public void SetButtonLabel(string label) => buttonText.text = label;
    }
}
