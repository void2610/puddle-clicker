using System;
using R3;

namespace PuddleClicker.Model
{
    public class GameModel : IDisposable
    {
        // しずく（通貨）
        private readonly ReactiveProperty<long> _drops = new(0);
        public ReadOnlyReactiveProperty<long> Drops => _drops;

        // クリックあたりの獲得量（初期値: 1）
        private readonly ReactiveProperty<int> _dropsPerClick = new(1);
        public ReadOnlyReactiveProperty<int> DropsPerClick => _dropsPerClick;

        // 毎秒の自動獲得量（初期値: 0）
        private readonly ReactiveProperty<float> _dropsPerSecond = new(0f);
        public ReadOnlyReactiveProperty<float> DropsPerSecond => _dropsPerSecond;

        public void AddDrops(long amount) => _drops.Value += amount;

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
