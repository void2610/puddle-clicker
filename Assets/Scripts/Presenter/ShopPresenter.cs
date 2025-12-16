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
            _shopView.Initialize(_upgradeModel.CompanionCount);

            InitializeDropItemView();
            InitializeCompanionViews();
            SubscribeToModelChanges();
        }

        private void InitializeDropItemView()
        {
            var view = _shopView.DropItemViews[0];
            UpdateDropItemView(view);
            view.OnActionClicked.Subscribe(_ => OnDropItemUpgrade()).AddTo(_disposables);
        }

        private void InitializeCompanionViews()
        {
            for (var i = 0; i < _shopView.CompanionViews.Count; i++)
            {
                var index = i;
                var view = _shopView.CompanionViews[i];
                view.Initialize(_upgradeModel.GetCompanionName(i), _upgradeModel.GetCompanionPrice(i), "購入");
                view.UpdateCount(_upgradeModel.GetCompanionCount(i));
                view.OnActionClicked.Subscribe(_ => OnCompanionPurchase(index)).AddTo(_disposables);
            }
        }

        private void SubscribeToModelChanges()
        {
            // 所持しずくが変わったらボタンの有効状態を更新
            _gameModel.Drops.Subscribe(_ => UpdateButtonStates()).AddTo(_disposables);
            // 落とすものレベルが変わったら表示を更新
            _upgradeModel.CurrentDropItemLevel.Subscribe(_ => UpdateDropItemViews()).AddTo(_disposables);
            // 仲間の数が変わったら表示を更新
            _upgradeModel.CompanionCounts.Subscribe(_ => UpdateCompanionViews()).AddTo(_disposables);
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

        private void OnCompanionPurchase(int index)
        {
            var price = _upgradeModel.GetCompanionPrice(index);
            if (_gameModel.TrySpendDrops(price))
            {
                _upgradeModel.AddCompanion(index);
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
            for (var i = 0; i < _shopView.CompanionViews.Count; i++)
            {
                var view = _shopView.CompanionViews[i];
                var price = _upgradeModel.GetCompanionPrice(i);
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
            var currentLevel = _upgradeModel.CurrentDropItemLevel.CurrentValue;
            var currentName = _upgradeModel.GetDropItemName(currentLevel);

            if (_upgradeModel.CanUpgradeDropItem())
            {
                var nextName = _upgradeModel.GetDropItemName(_upgradeModel.GetNextDropItemLevel()!.Value);
                var nextPrice = _upgradeModel.GetNextDropItemPrice();
                view.Initialize($"{currentName} → {nextName}", nextPrice, "強化");
            }
            else
            {
                view.Initialize($"{currentName}（最大）", 0, "最大");
                view.SetButtonInteractable(false);
            }
        }

        private void UpdateCompanionViews()
        {
            for (var i = 0; i < _shopView.CompanionViews.Count; i++)
            {
                var view = _shopView.CompanionViews[i];
                view.UpdateCount(_upgradeModel.GetCompanionCount(i));
                view.UpdatePrice(_upgradeModel.GetCompanionPrice(i));
            }
            UpdateButtonStates();
        }

        public void Dispose() => _disposables.Dispose();
    }
}
