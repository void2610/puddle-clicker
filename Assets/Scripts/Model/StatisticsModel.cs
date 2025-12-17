using System;
using R3;

namespace PuddleClicker.Model
{
    public sealed class StatisticsModel : IDisposable
    {
        public ReadOnlyReactiveProperty<long> TotalDropsEarned => _totalDropsEarned;
        public ReadOnlyReactiveProperty<long> TotalClicks => _totalClicks;
        public ReadOnlyReactiveProperty<float> PlayTime => _playTime;
        public ReadOnlyReactiveProperty<float> MaxDps => _maxDps;

        private readonly ReactiveProperty<long> _totalDropsEarned = new(0);
        private readonly ReactiveProperty<long> _totalClicks = new(0);
        private readonly ReactiveProperty<float> _playTime = new(0f);
        private readonly ReactiveProperty<float> _maxDps = new(0f);

        public void AddDropsEarned(long amount) => _totalDropsEarned.Value += amount;

        public void IncrementClicks() => _totalClicks.Value++;

        public void AddPlayTime(float deltaTime) => _playTime.Value += deltaTime;

        public void UpdateMaxDps(float dps)
        {
            if (dps > _maxDps.Value)
                _maxDps.Value = dps;
        }

        // セーブデータ復元用
        public void SetStatistics(long totalDrops, long totalClicks, float playTime, float maxDps)
        {
            _totalDropsEarned.Value = totalDrops;
            _totalClicks.Value = totalClicks;
            _playTime.Value = playTime;
            _maxDps.Value = maxDps;
        }

        public void Dispose()
        {
            _totalDropsEarned.Dispose();
            _totalClicks.Dispose();
            _playTime.Dispose();
            _maxDps.Dispose();
        }
    }
}
