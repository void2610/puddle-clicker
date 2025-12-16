using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PuddleClicker.Model;
using R3;
using UnityEngine;
using Void2610.UnityTemplate;

namespace PuddleClicker.Service
{
    public class SaveService : IDisposable
    {
        private const string SAVE_KEY = "puddle_clicker_save";
        private const float AUTO_SAVE_INTERVAL = 5f;

        private readonly GameModel _gameModel;
        private readonly UpgradeModel _upgradeModel;
        private readonly CompositeDisposable _disposables = new();
        private readonly CancellationTokenSource _cts = new();

        public SaveService(GameModel gameModel, UpgradeModel upgradeModel)
        {
            _gameModel = gameModel;
            _upgradeModel = upgradeModel;

            // ゲーム起動時にロード
            Load();

            // 購入時にオートセーブ
            _upgradeModel.CurrentDropItemLevel
                .Skip(1)
                .Subscribe(_ => Save())
                .AddTo(_disposables);

            _upgradeModel.CompanionCounts
                .Skip(1)
                .Subscribe(_ => Save())
                .AddTo(_disposables);

            // 定期オートセーブ開始
            StartAutoSave(_cts.Token).Forget();
        }

        private async UniTaskVoid StartAutoSave(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(AUTO_SAVE_INTERVAL), cancellationToken: ct);
                Save();
            }
        }

        private void Save()
        {
            var data = new SaveData
            {
                drops = _gameModel.Drops.CurrentValue,
                dropItemLevel = _upgradeModel.CurrentDropItemLevel.CurrentValue,
                companionCounts = _upgradeModel.CompanionCounts.CurrentValue
            };
            DataPersistence.SaveData(SAVE_KEY, JsonUtility.ToJson(data));
        }

        private void Load()
        {
            var json = DataPersistence.LoadData(SAVE_KEY);
            if (string.IsNullOrEmpty(json)) return;

            var data = JsonUtility.FromJson<SaveData>(json);

            // データ復元
            _gameModel.SetDrops(data.drops);
            _upgradeModel.SetDropItemLevel(data.dropItemLevel);
            _upgradeModel.SetCompanionCounts(data.companionCounts);

            // 派生値を再計算
            _gameModel.SetDropsPerClick(_upgradeModel.GetCurrentClickEffect());
            _gameModel.SetDropsPerSecond(_upgradeModel.GetTotalDropsPerSecond());
        }

        public void Dispose()
        {
            Save();
            _cts.Cancel();
            _cts.Dispose();
            _disposables.Dispose();
        }

        [Serializable]
        private class SaveData
        {
            public long drops;
            public int dropItemLevel;
            public int[] companionCounts;
        }
    }
}
