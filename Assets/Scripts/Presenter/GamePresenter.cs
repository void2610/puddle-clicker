using System;
using R3;
using UnityEngine;
using VContainer.Unity;
using PuddleClicker.Model;
using PuddleClicker.View;

namespace PuddleClicker.Presenter
{
    public class GamePresenter : IStartable, ITickable, IDisposable
    {
        private readonly GameModel _gameModel;
        private readonly PuddleView _puddleView;
        private readonly GameUIView _gameUIView;
        private readonly CompositeDisposable _disposables = new();

        // 自動生成用の蓄積時間
        private float _accumulatedTime;

        public GamePresenter(GameModel gameModel)
        {
            _gameModel = gameModel;
            _puddleView = UnityEngine.Object.FindFirstObjectByType<PuddleView>();
            _gameUIView = UnityEngine.Object.FindFirstObjectByType<GameUIView>();
        }

        public void Start()
        {
            // クリックイベントの購読
            _puddleView.OnClicked
                .Subscribe(OnPuddleClicked)
                .AddTo(_disposables);

            // しずく数の変更を購読してUIを更新
            _gameModel.Drops
                .Subscribe(drops => _gameUIView.UpdateDropsDisplay(drops))
                .AddTo(_disposables);

            // 毎秒獲得量の変更を購読してUIを更新
            _gameModel.DropsPerSecond
                .Subscribe(dps => _gameUIView.UpdateDropsPerSecondDisplay(dps))
                .AddTo(_disposables);
        }

        public void Tick()
        {
            // 自動生成処理
            var dps = _gameModel.DropsPerSecond.CurrentValue;
            if (dps <= 0) return;

            _accumulatedTime += Time.deltaTime;

            // 1秒ごとにしずくを加算
            if (_accumulatedTime >= 1f)
            {
                var dropsToAdd = (long)(_accumulatedTime * dps);
                _gameModel.AddDrops(dropsToAdd);
                _accumulatedTime %= 1f;
            }
        }

        private void OnPuddleClicked(Vector3 position)
        {
            // しずくを獲得
            var amount = _gameModel.DropsPerClick.CurrentValue;
            _gameModel.AddDrops(amount);

            // 波紋エフェクトを再生
            _puddleView.PlayRippleEffect(position);
        }

        public void Dispose() => _disposables.Dispose();
    }
}
