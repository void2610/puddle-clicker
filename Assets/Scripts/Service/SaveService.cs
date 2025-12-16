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

        public long PendingOfflineReward { get; private set; }
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
                companionCounts = _upgradeModel.CompanionCounts.CurrentValue,
                lastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            DataPersistence.SaveData(SAVE_KEY, JsonUtility.ToJson(data));
        }

        private void Load()
        {
            var json = DataPersistence.LoadData(SAVE_KEY);
            if (string.IsNullOrEmpty(json)) return;

            var data = JsonUtility.FromJson<SaveData>(json);

            // データ復元
            _upgradeModel.SetDropItemLevel(data.dropItemLevel);
            _upgradeModel.SetCompanionCounts(data.companionCounts);

            // 派生値を再計算
            _gameModel.SetDropsPerClick(_upgradeModel.GetCurrentClickEffect());
            _gameModel.SetDropsPerSecond(_upgradeModel.GetTotalDropsPerSecond());

            // 放置報酬を計算・付与
            var offlineReward = CalculateOfflineReward(data.lastSaveTimestamp);
            _gameModel.SetDrops(data.drops + offlineReward);
            PendingOfflineReward = offlineReward;
        }

        private long CalculateOfflineReward(long lastSaveTimestamp)
        {
            if (lastSaveTimestamp <= 0) return 0;

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var elapsedSeconds = now - lastSaveTimestamp;
            var dropsPerSecond = _upgradeModel.GetTotalDropsPerSecond();

            return (long)(elapsedSeconds * dropsPerSecond);
        }

        public void Dispose()
        {
            Save();
            _cts.Cancel();
            _cts.Dispose();
            _disposables.Dispose();
        }

        [Serializable]
        private sealed class SaveData
        {
            public long drops;
            public int dropItemLevel;
            public int[] companionCounts;
            public long lastSaveTimestamp;
        }
    }
}
