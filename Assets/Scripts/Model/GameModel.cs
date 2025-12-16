using System;
using R3;

namespace PuddleClicker.Model
{
    public class GameModel : IDisposable
    {
        public ReadOnlyReactiveProperty<long> Drops => _drops;
        public ReadOnlyReactiveProperty<int> DropsPerClick => _dropsPerClick;
        public ReadOnlyReactiveProperty<float> DropsPerSecond => _dropsPerSecond;

        private readonly ReactiveProperty<long> _drops = new(0);
        private readonly ReactiveProperty<int> _dropsPerClick = new(1);
        private readonly ReactiveProperty<float> _dropsPerSecond = new(0f);

        public void AddDrops(long amount) => _drops.Value += amount;

        public void SetDrops(long value) => _drops.Value = value;

        public bool TrySpendDrops(long amount)
        {
            if (_drops.Value < amount) return false;
            _drops.Value -= amount;
            return true;
        }

        public void SetDropsPerClick(int value) => _dropsPerClick.Value = value;

        public void SetDropsPerSecond(float value) => _dropsPerSecond.Value = value;

        public void Dispose()
        {
            _drops.Dispose();
            _dropsPerClick.Dispose();
            _dropsPerSecond.Dispose();
        }
    }
}
