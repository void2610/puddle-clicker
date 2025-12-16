using UnityEngine;
using TMPro;

namespace PuddleClicker.View
{
    public class GameUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dropsText;
        [SerializeField] private TextMeshProUGUI dropsPerSecondText;

        public void UpdateDropsDisplay(long drops)
        {
            dropsText.text = drops.ToString("N0");
        }

        public void UpdateDropsPerSecondDisplay(float dropsPerSecond)
        {
            dropsPerSecondText.text = $"{dropsPerSecond:F1}/秒";
        }
    }
}
