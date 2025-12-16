using System;
using System.Collections.Generic;
using System.Linq;
using R3;

namespace PuddleClicker.Model
{
    public class UpgradeModel : IDisposable
    {
        // 現在の落とすものレベル（最高レベルが自動適用）
        public ReadOnlyReactiveProperty<DropItemType> CurrentDropItem => _currentDropItem;
        public ReadOnlyReactiveProperty<Dictionary<CompanionType, int>> CompanionCounts => _companionCounts;

        private readonly ReactiveProperty<DropItemType> _currentDropItem;
        private readonly ReactiveProperty<Dictionary<CompanionType, int>> _companionCounts;

        public UpgradeModel()
        {
            // 初期状態: 指先
            _currentDropItem = new ReactiveProperty<DropItemType>(DropItemType.Finger);

            // 初期状態: 仲間は0匹
            var initialCounts = new Dictionary<CompanionType, int>();
            foreach (CompanionType type in Enum.GetValues(typeof(CompanionType)))
            {
                initialCounts[type] = 0;
            }
            _companionCounts = new ReactiveProperty<Dictionary<CompanionType, int>>(initialCounts);
        }

        public bool CanUpgradeDropItem()
        {
            var currentLevel = (int)_currentDropItem.Value;
            return currentLevel < UpgradeDefinitions.DropItems.Length - 1;
        }

        public DropItemType? GetNextDropItem()
        {
            if (!CanUpgradeDropItem()) return null;
            return (DropItemType)((int)_currentDropItem.Value + 1);
        }

        public long GetNextDropItemPrice()
        {
            var next = GetNextDropItem();
            if (next == null) return 0;
            return UpgradeDefinitions.DropItems[(int)next.Value].Price;
        }

        public void UpgradeDropItem()
        {
            if (!CanUpgradeDropItem()) return;
            _currentDropItem.Value = (DropItemType)((int)_currentDropItem.Value + 1);
        }

        public int GetCurrentClickEffect() => UpgradeDefinitions.DropItems[(int)_currentDropItem.Value].Effect;

        public int GetCompanionCount(CompanionType type) => _companionCounts.Value.TryGetValue(type, out var count) ? count : 0;

        public void AddCompanion(CompanionType type)
        {
            var newDict = new Dictionary<CompanionType, int>(_companionCounts.Value);
            newDict[type] = GetCompanionCount(type) + 1;
            _companionCounts.Value = newDict;
        }

        public float GetTotalDropsPerSecond()
        {
            float total = 0f;
            foreach (var companion in UpgradeDefinitions.Companions)
            {
                var count = GetCompanionCount(companion.Type);
                total += companion.Effect * count;
            }
            return total;
        }

        public long GetCompanionPrice(CompanionType type)
        {
            var basePrice = UpgradeDefinitions.Companions.First(x => x.Type == type).BasePrice;
            var count = GetCompanionCount(type);
            return (long)(basePrice * Math.Pow(UpgradeDefinitions.PriceMultiplier, count));
        }

        public void Dispose()
        {
            _currentDropItem.Dispose();
            _companionCounts.Dispose();
        }
    }
}
