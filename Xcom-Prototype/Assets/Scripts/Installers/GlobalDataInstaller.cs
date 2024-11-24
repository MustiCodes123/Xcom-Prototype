using System.Collections.Generic;
using Zenject;

public class GlobalDataInstaller : Installer<GlobalDataInstaller>
{
    [Inject] private SaveManager _saveManager;

    public override void InstallBindings()
    {
        Container.Bind<PlayerData>().FromInstance(_saveManager.InitPlayerData(false)).AsSingle().NonLazy();
        Container.Bind<PvPPlayerService>().FromInstance(_saveManager.LoadPvPData()).AsSingle().NonLazy();
    }
}