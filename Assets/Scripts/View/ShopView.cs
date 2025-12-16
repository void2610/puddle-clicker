using System.Collections.Generic;
using UnityEngine;

namespace PuddleClicker.View
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private ShopItemView itemPrefab;
        [SerializeField] private Transform dropItemContainer;
        [SerializeField] private Transform companionContainer;

        public IReadOnlyList<ShopItemView> DropItemViews => _dropItemViews;
        public IReadOnlyList<ShopItemView> CompanionViews => _companionViews;

        private readonly List<ShopItemView> _dropItemViews = new();
        private readonly List<ShopItemView> _companionViews = new();

        public void Initialize(int companionCount)
        {
            GenerateDropItemViews();
            GenerateCompanionViews(companionCount);
        }

        private void GenerateDropItemViews()
        {
            // 落とすものは1つのビューのみ（アップグレード表示用）
            var view = Instantiate(itemPrefab, dropItemContainer);
            _dropItemViews.Add(view);
        }

        private void GenerateCompanionViews(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var view = Instantiate(itemPrefab, companionContainer);
                _companionViews.Add(view);
            }
        }
    }
}
