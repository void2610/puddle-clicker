using System;
using PuddleClicker.Model;
using PuddleClicker.View;
using R3;

namespace PuddleClicker.Presenter
{
    public sealed class StatisticsPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        public StatisticsPresenter(StatisticsModel statisticsModel)
        {
            var statisticsView = UnityEngine.Object.FindFirstObjectByType<StatisticsView>();

            // 開くボタンのイベント購読
            statisticsView.OnOpenClicked
                .Subscribe(_ => statisticsView.Open())
                .AddTo(_disposables);

            // いずれかの統計値が変更されたら全体を更新
            Observable.CombineLatest(
                    statisticsModel.TotalDropsEarned,
                    statisticsModel.TotalClicks,
                    statisticsModel.PlayTime,
                    statisticsModel.MaxDps,
                    (drops, clicks, time, dps) => (drops, clicks, time, dps))
                .Subscribe(x => statisticsView.UpdateDisplay(x.drops, x.clicks, x.time, x.dps))
                .AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();
    }
}
