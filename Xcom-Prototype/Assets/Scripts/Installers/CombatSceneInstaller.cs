using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CombatSceneInstaller : MonoInstaller
{
    public CombatSpawner CombatSpawner;
    public BattleController BattleController;

    [SerializeField] private UIWindowManager windowManager;
    [SerializeField] private UIBattleWindow _battleWindow;
    [SerializeField] private UIBottomPanel bottomPanel;
    [SerializeField] private Tooltip Tooltip;
    [SerializeField] private InfoPopup InfoPopup;
    [SerializeField] private CustomGameTimeScaleHandler _customGameTimeScaleHandler;

    public override void InstallBindings()
    {
        Container.Bind<UIBottomPanel>().FromInstance(bottomPanel).AsSingle();
        Container.Bind<UIWindowManager>().FromInstance(windowManager).AsSingle();
        Container.Bind<Tooltip>().FromInstance(Tooltip).AsSingle();
        Container.Bind<InfoPopup>().FromInstance(InfoPopup).AsSingle();

        Container.BindFactory<BaseCharacterModel, BaseCharacerView, BaseCharacerView.Factory>().FromFactory<CharacterFactory>().NonLazy();

        var characterRegistry = new CharactersRegistry(windowManager, Container.Resolve<BaseCharacerView.Factory>(), CombatSpawner, _battleWindow);

        if (BattleController != null)
            Container.Bind<BattleController>().FromInstance(BattleController).AsSingle().NonLazy();

        Container.Bind<CharactersRegistry>().FromInstance(characterRegistry).AsSingle().NonLazy();

        Container.BindFactory<string, Transform, CombatText, CombatText.Factory>()
            .FromPoolableMemoryPool<string, Transform, CombatText, CombatTextPool>(poolBinder => poolBinder
                           .WithInitialSize(25)
                                          .FromComponentInNewPrefabResource("Prefabs/UI/CombatText")
                                                         .UnderTransformGroup("CombatText"));

        Container.BindFactory<CampLevelEnemies, IEnemy, EnemyFactory>().FromFactory<EnemyCharacterFactory>();

        var itemFactory = Container.BindFactory<string, Task<ItemView>, ItemView.Factory>().FromFactory<ItemViewFactory>().NonLazy();

        var decaleFactory = Container.BindFactory<DecaleType, BaseDecale, BaseDecale.Factory>().FromFactory<DecaleFactory>().NonLazy();

        var partilceFactory = Container.BindFactory<ParticleType, BaseParticleView, BaseParticleView.Factory>().FromFactory<ParticleFactory>().NonLazy();

        var projectileFactory = Container.BindFactory<ProjectileType, BaseProjectile, BaseProjectile.Factory>().FromFactory<ProjectileFactory>().NonLazy();

        var weaponProjectileFactory = Container.BindFactory<RangeWeaponView, BaseShootProjectile, BaseShootProjectile.Factory>().FromFactory<WeaponProjectileFactory>().NonLazy();

        var talentFactory = new TalentFactory(Container.Resolve<BaseParticleView.Factory>(), characterRegistry, Container.Resolve<BaseProjectile.Factory>(), Container.Resolve<BaseDecale.Factory>());

        Container.Bind<TalentFactory>().FromInstance(talentFactory).AsSingle();
        
        Container.BindInterfacesTo<ShapeMathImpl>().AsSingle();
        
        Container.Bind<CustomGameTimeScaleHandler>().FromInstance(_customGameTimeScaleHandler).AsSingle();

    }

    private class CombatTextPool : MonoPoolableMemoryPool<string, Transform, IMemoryPool, CombatText>
    {

    }
}
