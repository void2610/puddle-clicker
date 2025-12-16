using UnityEngine;
using VContainer;
using VContainer.Unity;
using PuddleClicker.Model;
using PuddleClicker.Presenter;
using PuddleClicker.Service;

public class MainLifetimeScope : LifetimeScope
{
    [SerializeField] private DropItemSettings dropItemSettings;
    [SerializeField] private CompanionSettings companionSettings;
    [SerializeField] private GameBalanceSettings gameBalanceSettings;

    protected override void Configure(IContainerBuilder builder)
    {
        // ScriptableObject登録
        builder.RegisterInstance(dropItemSettings);
        builder.RegisterInstance(companionSettings);
        builder.RegisterInstance(gameBalanceSettings);

        // Model登録
        builder.Register<GameModel>(Lifetime.Singleton);
        builder.Register<UpgradeModel>(Lifetime.Singleton);

        // Service登録
        builder.Register<SaveService>(Lifetime.Singleton);

        // Presenter登録（エントリーポイント）
        builder.RegisterEntryPoint<GamePresenter>().AsSelf();
        builder.Register<ShopPresenter>(Lifetime.Singleton);
        builder.Register<CompanionPresenter>(Lifetime.Singleton);

        builder.RegisterEntryPoint<GameInitializer>();
    }
}
