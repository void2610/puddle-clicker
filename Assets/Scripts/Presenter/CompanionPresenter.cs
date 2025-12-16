using System;
using PuddleClicker.Model;
using PuddleClicker.View;
using R3;
using UnityEngine;

namespace PuddleClicker.Presenter
{
    public class CompanionPresenter : IDisposable
    {
        private readonly UpgradeModel _upgradeModel;
        private readonly CompanionSettings _companionSettings;
        private readonly CompanionView _companionView;
        private readonly CompositeDisposable _disposables = new();

        // 前回の仲間カウントを保持
        private int[] _previousCounts;

        public CompanionPresenter(UpgradeModel upgradeModel, CompanionSettings companionSettings)
        {
            _upgradeModel = upgradeModel;
            _companionSettings = companionSettings;
            _companionView = UnityEngine.Object.FindFirstObjectByType<CompanionView>();

            _previousCounts = new int[companionSettings.Count];

            // 仲間カウントの変更を監視
            _upgradeModel.CompanionCounts
                .Subscribe(OnCompanionCountsChanged)
                .AddTo(_disposables);
        }

        private void OnCompanionCountsChanged(int[] newCounts)
        {
            for (var i = 0; i < newCounts.Length; i++)
            {
                // 増加した分だけ仲間をスポーン
                var diff = newCounts[i] - _previousCounts[i];
                for (var j = 0; j < diff; j++)
                {
                    var companionData = _companionSettings.Companions[i];
                    _companionView.SpawnCompanion(companionData);
                }
                _previousCounts[i] = newCounts[i];
            }
        }

        public void Dispose() => _disposables.Dispose();
    }
}
