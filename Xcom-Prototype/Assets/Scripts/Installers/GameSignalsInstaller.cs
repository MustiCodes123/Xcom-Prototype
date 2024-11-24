using Signals;
using UnityEngine;
using Zenject;

public class GameSignalsInstaller : Installer<GameSignalsInstaller>
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<MoneyChangeSignal>();
        Container.DeclareSignal<CharacterSelectSignal>();
        Container.DeclareSignal<CharacterLevelpSignal>();
        Container.DeclareSignal<CharacterEquipSignal>();
        Container.DeclareSignal<CharacterAddedToGroupSignal>();
        Container.DeclareSignal<ItemBuySignal>();
        Container.DeclareSignal<LevelFinishSignal>();
        Container.DeclareSignal<QuestCompleteSignal>();
        Container.DeclareSignal<SummonHeroSignal>();
        Container.DeclareSignal<UpgradeSignal>();
        Container.DeclareSignal<UseResourceSignal>();
        Container.DeclareSignal<ShopWindowOpenSignal>();
        Container.DeclareSignal<OpenWindowSignal>();
        Container.DeclareSignal<ChangeGameStateSignal>();
        Container.DeclareSignal<ChangeGameSpeedSignal>();

        Container.Bind<MoneyChangeHandler>().AsSingle().NonLazy();
        Container.Bind<CharacterHandler>().AsSingle().NonLazy();
        Container.Bind<ShopHundler>().AsSingle().NonLazy();
        Container.Bind<LevelFinishHandler>().AsSingle().NonLazy();
        Container.Bind<PlayerAnalyser>().AsSingle().NonLazy();
        
    }
}

