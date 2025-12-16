using System;
using R3;
using PuddleClicker.Model;
using PuddleClicker.View;

namespace PuddleClicker.Presenter
{
    public class ShopPresenter : IDisposable
    {
        private readonly GameModel _gameModel;
        private readonly UpgradeModel _upgradeModel;
        private readonly ShopView _shopView;
        private readonly CompositeDisposable _disposables = new();

        public ShopPresenter(GameModel gameModel, UpgradeModel upgradeModel)
        {
            _gameModel = gameModel;
            _upgradeModel = upgradeModel;
            _shopView = UnityEngine.Object.FindFirstObjectByType<ShopView>();
            _shopView.Initialize();

            InitializeDropItemView();
            InitializeCompanionViews();
            SubscribeToModelChanges();
        }

        private void InitializeDropItemView()
        {
            // 落とすものは1つのビューでアップグレード表示
            var view = _shopView.DropItemViews[0];
            UpdateDropItemView(view);

            view.OnActionClicked
                .Subscribe(_ => OnDropItemUpgrade())
                .AddTo(_disposables);
        }

        private void InitializeCompanionViews()
        {
            for (var i = 0; i < _shopView.CompanionViews.Count && i < UpgradeDefinitions.Companions.Length; i++)
            {
                var view = _shopView.CompanionViews[i];
                var companion = UpgradeDefinitions.Companions[i];
                var companionType = companion.Type;

                view.Initialize(i, companion.Name, _upgradeModel.GetCompanionPrice(companionType), "購入");
                view.UpdateCount(_upgradeModel.GetCompanionCount(companionType));

                view.OnActionClicked
                    .Subscribe(_ => OnCompanionPurchase(companionType))
                    .AddTo(_disposables);
            }
        }

        private void SubscribeToModelChanges()
        {
            // 所持しずくが変わったらボタンの有効状態を更新
            _gameModel.Drops
                .Subscribe(_ => UpdateButtonStates())
                .AddTo(_disposables);

            // 落とすものレベルが変わったら表示を更新
            _upgradeModel.CurrentDropItem
                .Subscribe(_ => UpdateDropItemViews())
                .AddTo(_disposables);

            // 仲間の数が変わったら表示を更新
            _upgradeModel.CompanionCounts
                .Subscribe(_ => UpdateCompanionViews())
                .AddTo(_disposables);
        }

        private void OnDropItemUpgrade()
        {
            var price = _upgradeModel.GetNextDropItemPrice();
            if (_gameModel.TrySpendDrops(price))
            {
                _upgradeModel.UpgradeDropItem();
                _gameModel.SetDropsPerClick(_upgradeModel.GetCurrentClickEffect());
            }
        }

        private void OnCompanionPurchase(CompanionType type)
        {
            var price = _upgradeModel.GetCompanionPrice(type);
            if (_gameModel.TrySpendDrops(price))
            {
                _upgradeModel.AddCompanion(type);
                _gameModel.SetDropsPerSecond(_upgradeModel.GetTotalDropsPerSecond());
            }
        }

        private void UpdateButtonStates()
        {
            var drops = _gameModel.Drops.CurrentValue;

            // 落とすものボタン
            var dropItemView = _shopView.DropItemViews[0];
            var canUpgrade = _upgradeModel.CanUpgradeDropItem();
            var dropItemPrice = _upgradeModel.GetNextDropItemPrice();
            dropItemView.SetButtonInteractable(canUpgrade && drops >= dropItemPrice);

            // 仲間ボタン
            for (var i = 0; i < _shopView.CompanionViews.Count && i < UpgradeDefinitions.Companions.Length; i++)
            {
                var view = _shopView.CompanionViews[i];
                var companion = UpgradeDefinitions.Companions[i];
                var price = _upgradeModel.GetCompanionPrice(companion.Type);
                view.SetButtonInteractable(drops >= price);
            }
        }

        private void UpdateDropItemViews()
        {
            UpdateDropItemView(_shopView.DropItemViews[0]);
            UpdateButtonStates();
        }

        private void UpdateDropItemView(ShopItemView view)
        {
            var current = _upgradeModel.CurrentDropItem.CurrentValue;
            var currentName = UpgradeDefinitions.DropItems[(int)current].Name;

            if (_upgradeModel.CanUpgradeDropItem())
            {
                var next = _upgradeModel.GetNextDropItem()!.Value;
                var nextName = UpgradeDefinitions.DropItems[(int)next].Name;
                var nextPrice = _upgradeModel.GetNextDropItemPrice();

                view.Initialize(0, $"{currentName} → {nextName}", nextPrice, "強化");
            }
            else
            {
                view.Initialize(0, $"{currentName}（最大）", 0, "最大");
                view.SetButtonInteractable(false);
            }
        }

        private void UpdateCompanionViews()
        {
            for (var i = 0; i < _shopView.CompanionViews.Count && i < UpgradeDefinitions.Companions.Length; i++)
            {
                var view = _shopView.CompanionViews[i];
                var companion = UpgradeDefinitions.Companions[i];
                var count = _upgradeModel.GetCompanionCount(companion.Type);
                var price = _upgradeModel.GetCompanionPrice(companion.Type);

                view.UpdateCount(count);
                view.UpdatePrice(price);
            }

            UpdateButtonStates();
        }

        public void Dispose() => _disposables.Dispose();
    }
}
