using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PuddleClicker.View
{
    public class StatisticsView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI statisticsText;

        public Observable<Unit> OnOpenClicked => openButton.OnClickAsObservable();

        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
            panel.SetActive(false);
        }

        public void Open() => panel.SetActive(true);

        private void Close() => panel.SetActive(false);

        public void UpdateDisplay(long totalDrops, long totalClicks, float playTimeSeconds, float maxDps)
        {
            var time = TimeSpan.FromSeconds(playTimeSeconds);
            statisticsText.text = $"累計獲得しずく: {totalDrops:N0}\n" +
                                  $"累計クリック数: {totalClicks:N0}\n" +
                                  $"プレイ時間: {(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}\n" +
                                  $"最高DPS: {maxDps:F1}/秒";
        }
    }
}
