using Zenject;

public class DMInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<DMItemsDataProvider>().AsSingle().NonLazy();
    }
}
