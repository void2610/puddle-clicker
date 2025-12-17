using PuddleClicker.Presenter;
using PuddleClicker.Service;
using PuddleClicker.View;
using UnityEngine;
using VContainer.Unity;

public class GameInitializer : IStartable
{
    private readonly SaveService _saveService;

    public GameInitializer(SaveService saveService, GamePresenter gamePresenter, ShopPresenter shopPresenter, CompanionPresenter companionPresenter, StatisticsPresenter statisticsPresenter)
    {
        _saveService = saveService;
    }

    public void Start()
    {
        // 放置報酬がある場合はUIを表示
        if (_saveService.PendingOfflineReward > 0)
        {
            var offlineRewardView = Object.FindFirstObjectByType<OfflineRewardView>();
            offlineRewardView.Show(_saveService.PendingOfflineReward);
        }
    }
}