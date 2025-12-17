using System;
using R3;
using UnityEngine;
using VContainer.Unity;
using PuddleClicker.Model;
using PuddleClicker.View;

namespace PuddleClicker.Presenter
{
    public class GamePresenter : ITickable, IDisposable
    {
        private readonly GameModel _gameModel;
        private readonly UpgradeModel _upgradeModel;
        private readonly StatisticsModel _statisticsModel;
        private readonly PuddleView _puddleView;
        private readonly CompositeDisposable _disposables = new();

        // 自動生成用の蓄積時間
        private float _accumulatedTime;

        public GamePresenter(GameModel gameModel, UpgradeModel upgradeModel, StatisticsModel statisticsModel)
        {
            _gameModel = gameModel;
            _upgradeModel = upgradeModel;
            _statisticsModel = statisticsModel;
            _puddleView = UnityEngine.Object.FindFirstObjectByType<PuddleView>();
            var gameUIView = UnityEngine.Object.FindFirstObjectByType<GameUIView>();

            // 操作イベントの購読
            _puddleView.OnClicked.Subscribe(OnPuddleClicked).AddTo(_disposables);
            // UI更新
            _gameModel.Drops.Subscribe(drops => gameUIView.UpdateDropsDisplay(drops)).AddTo(_disposables);
            _gameModel.DropsPerSecond.Subscribe(dps => gameUIView.UpdateDropsPerSecondDisplay(dps)).AddTo(_disposables);

            // 最高DPS更新
            _gameModel.DropsPerSecond
                .Subscribe(dps => _statisticsModel.UpdateMaxDps(dps))
                .AddTo(_disposables);
        }

        public void Tick()
        {
            // プレイ時間更新
            _statisticsModel.AddPlayTime(Time.deltaTime);

            // 自動生成処理
            var dps = _gameModel.DropsPerSecond.CurrentValue;
            if (dps <= 0) return;

            _accumulatedTime += Time.deltaTime;

            // 1秒ごとにしずくを加算
            if (_accumulatedTime >= 1f)
            {
                var dropsToAdd = (long)(_accumulatedTime * dps);
                _gameModel.AddDrops(dropsToAdd);
                _statisticsModel.AddDropsEarned(dropsToAdd);
                _accumulatedTime %= 1f;
            }
        }

        private void OnPuddleClicked(Vector3 position)
        {
            // しずくを獲得
            var amount = _gameModel.DropsPerClick.CurrentValue;
            _gameModel.AddDrops(amount);

            // 統計更新
            _statisticsModel.IncrementClicks();
            _statisticsModel.AddDropsEarned(amount);

            // 現在の落とすものの波紋パラメータを取得
            var dropItem = _upgradeModel.GetCurrentDropItem();
            _puddleView.PlayRippleEffect(position, dropItem.RippleScale, dropItem.RippleDuration, dropItem.RippleCount);
        }

        public void Dispose() => _disposables.Dispose();
    }
}
