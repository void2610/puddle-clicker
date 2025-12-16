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

        public void Initialize(string itemName, long price, string buttonLabel)
        {
            nameText.text = itemName;
            UpdatePrice(price);
            countText.gameObject.SetActive(false);
            buttonText.text = buttonLabel;
        }

        public void SetButtonInteractable(bool interactable) => actionButton.interactable = interactable;
        public void UpdatePrice(long price) => priceText.text = price > 0 ? $"{price:N0}" : "所持";

        public void UpdateCount(int count)
        {
            countText.gameObject.SetActive(true);
            countText.text = $"×{count}";
        }
    }
}
