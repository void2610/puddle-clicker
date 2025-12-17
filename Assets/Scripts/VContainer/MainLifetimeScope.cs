using UnityEngine;
using VContainer;
using VContainer.Unity;
using PuddleClicker.Model;
using PuddleClicker.Presenter;
using PuddleClicker.Service;
using Void2610.SettingsSystem;

public class MainLifetimeScope : LifetimeScope
{
    [SerializeField] private DropItemSettings dropItemSettings;
    [SerializeField] private CompanionSettings companionSettings;
    [SerializeField] private GameBalanceSettings gameBalanceSettings;
    
    [Header("デバッグ")]
    [SerializeField] private bool clearSaveOnStart; 

    protected override void Configure(IContainerBuilder builder)
    {
        // セーブデータ削除
        if (clearSaveOnStart)
        {
            DataPersistence.DeleteData("puddle_clicker_save"); 
            Debug.Log("[MainLifetimeScope] セーブデータを削除しました");
        }       
        
        // ScriptableObject登録
        builder.RegisterInstance(dropItemSettings);
        builder.RegisterInstance(companionSettings);
        builder.RegisterInstance(gameBalanceSettings);

        // Model登録
        builder.Register<GameModel>(Lifetime.Singleton);
        builder.Register<UpgradeModel>(Lifetime.Singleton);
        builder.Register<StatisticsModel>(Lifetime.Singleton);

        // Service登録
        builder.Register<SaveService>(Lifetime.Singleton);

        // Presenter登録（エントリーポイント）
        builder.Register<ShopPresenter>(Lifetime.Singleton);
        builder.Register<CompanionPresenter>(Lifetime.Singleton);
        builder.Register<StatisticsPresenter>(Lifetime.Singleton);

        builder.RegisterEntryPoint<GamePresenter>().AsSelf();
        builder.RegisterEntryPoint<GameInitializer>();
    }
}
