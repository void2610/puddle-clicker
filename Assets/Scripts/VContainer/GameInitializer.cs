using PuddleClicker.Presenter;
using PuddleClicker.Service;
using VContainer.Unity;

public class GameInitializer : IStartable
{
    public GameInitializer(SaveService saveService, GamePresenter gamePresenter, ShopPresenter shopPresenter, CompanionPresenter companionPresenter)
    {
    }

    public void Start()
    {
    }
}