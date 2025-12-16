using VContainer;
using VContainer.Unity;
using PuddleClicker.Model;
using PuddleClicker.Presenter;

public class MainLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // Model登録
        builder.Register<GameModel>(Lifetime.Singleton);

        // Presenter登録（エントリーポイント）
        builder.RegisterEntryPoint<GamePresenter>();
    }
}
