using System;
using R3;

namespace PuddleClicker.Model
{
    public class UpgradeModel : IDisposable
    {
        public ReadOnlyReactiveProperty<int> CurrentDropItemLevel => _currentDropItemLevel;
        public ReadOnlyReactiveProperty<int[]> CompanionCounts => _companionCounts;

        private readonly DropItemSettings _dropItemSettings;
        private readonly CompanionSettings _companionSettings;
        private readonly GameBalanceSettings _balanceSettings;
        private readonly ReactiveProperty<int> _currentDropItemLevel;
        private readonly ReactiveProperty<int[]> _companionCounts;

        public UpgradeModel(DropItemSettings dropItemSettings, CompanionSettings companionSettings, GameBalanceSettings balanceSettings)
        {
            _dropItemSettings = dropItemSettings;
            _companionSettings = companionSettings;
            _balanceSettings = balanceSettings;
            _currentDropItemLevel = new ReactiveProperty<int>(0);
            _companionCounts = new ReactiveProperty<int[]>(new int[companionSettings.Count]);
        }

        public bool CanUpgradeDropItem() => _currentDropItemLevel.Value < _dropItemSettings.Count - 1;

        public int? GetNextDropItemLevel() => CanUpgradeDropItem() ? _currentDropItemLevel.Value + 1 : null;

        public long GetNextDropItemPrice()
        {
            var next = GetNextDropItemLevel();
            return next == null ? 0 : _dropItemSettings.Items[next.Value].Price;
        }

        public void UpgradeDropItem()
        {
            if (CanUpgradeDropItem())
                _currentDropItemLevel.Value++;
        }

        public int GetCurrentClickEffect() => _dropItemSettings.Items[_currentDropItemLevel.Value].Effect;

        public string GetDropItemName(int level) => _dropItemSettings.Items[level].Name;

        public DropItemData GetCurrentDropItem() => _dropItemSettings.Items[_currentDropItemLevel.Value];

        public int GetCompanionCount(int index) => _companionCounts.Value[index];

        public void AddCompanion(int index)
        {
            var newCounts = (int[])_companionCounts.Value.Clone();
            newCounts[index]++;
            _companionCounts.Value = newCounts;
        }

        public float GetTotalDropsPerSecond()
        {
            var total = 0f;
            for (var i = 0; i < _companionSettings.Count; i++)
                total += _companionSettings.Companions[i].Effect * _companionCounts.Value[i];
            return total;
        }

        public long GetCompanionPrice(int index)
        {
            var basePrice = _companionSettings.Companions[index].BasePrice;
            var count = _companionCounts.Value[index];
            return (long)(basePrice * Math.Pow(_balanceSettings.PriceMultiplier, count));
        }

        public string GetCompanionName(int index) => _companionSettings.Companions[index].Name;

        public int CompanionCount => _companionSettings.Count;

        public void Dispose()
        {
            _currentDropItemLevel.Dispose();
            _companionCounts.Dispose();
        }
    }
}
