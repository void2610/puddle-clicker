using PuddleClicker.Presenter;
using VContainer;
using VContainer.Unity;

public class GameInitializer: IStartable
{
    [Inject]
    public GameInitializer(GamePresenter presenter)
    {
        // コンストラクタで依存関係の注入が必要な場合はここに記述
    }
    
    public void Start()
    {
        // 初期化処理が必要な場合はここに記述
    }
}