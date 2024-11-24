using System.ComponentModel;
using Data.Resources.AddressableManagement;
using Data.Resources.AddressableManagement.GameResourcesLoaders;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameInstaller", menuName = "Installers/GameInstaller")]
public class GameInstaller : ScriptableObjectInstaller<GameInstaller>
{
    [SerializeField] private ItemsDataInfo _itemsDataInfo;
    [SerializeField] private CharacterPresetsRegister _CharacterPresetsRegister;
    [SerializeField] private SetItemsContainer _setItemsContainer;
    [SerializeField] private SkillsDataInfo _skillsDataInfo;
    [SerializeField] private BattleCampInfo _battleCampInfo;
    [SerializeField] private QuestManager _questManager;
    [SerializeField] private ShopDataInfo  _shopDataInfo;
    [SerializeField] private ModeInfo _modeInfo;
    [SerializeField] private PvPBattleData _pvpData;
    [SerializeField] private ThreeToOneContainer _threeToOneData;
    [SerializeField] private ResourceManager _resourceManager;
    [SerializeField] private CharacterUpgradePrices _characterUpgradePrices;
    [SerializeField] private FeatureLockConfig _featureLockConfig;
    [SerializeField] private AddressableGroupsDataContainer _addressableDataContainer;

    public override void InstallBindings()
    {
        GameSignalsInstaller.Install(Container);

        Container.Bind<SkillsDataInfo>().FromInstance(_skillsDataInfo).AsSingle().NonLazy();   

        Container.Bind<CharacterPresetsRegister>().FromInstance(_CharacterPresetsRegister).AsSingle().NonLazy();   

        Container.Bind<SaveManager>().FromInstance(new SaveManager(_itemsDataInfo, _pvpData, _threeToOneData)).AsSingle().NonLazy();
  
        GlobalDataInstaller.Install(Container);

        Container.Bind<ItemsDataInfo>().FromInstance(_itemsDataInfo).AsSingle().NonLazy();
        Container.Bind<SetItemsContainer>().FromInstance(_setItemsContainer).AsSingle().NonLazy();
        Container.Bind<ShopDataInfo>().FromInstance(_shopDataInfo).AsSingle().NonLazy();
        Container.Bind<BattleCampInfo>().FromInstance(_battleCampInfo).AsSingle().NonLazy();
        Container.Bind<ModeInfo>().FromInstance(_modeInfo).AsSingle().NonLazy();
        Container.Bind<PvPBattleData>().FromInstance(_pvpData).AsSingle().NonLazy();
        Container.Bind<ThreeToOneContainer>().FromInstance(_threeToOneData).AsSingle().NonLazy();
        Container.Bind<UniRxDisposable>().FromInstance(new UniRxDisposable()).AsSingle().NonLazy();
        Container.Bind<FeatureLockConfig>().FromInstance(_featureLockConfig).AsSingle().NonLazy();

        Container.Bind<ResourceManager>().FromComponentInNewPrefab(_resourceManager).AsSingle().NonLazy();

        Container.Bind<QuestManager>().FromComponentInNewPrefab(_questManager).AsSingle().NonLazy();

        Container.Bind<ShopController>().AsSingle();

        Container.Bind<ShopRepository>().AsSingle();

        Container.Bind<TemploaryInfo>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Wallet wallet = Container.Instantiate<Wallet>();
        Container.Bind<Wallet>().FromMethod(context =>
        {
            Wallet wallet = Wallet.Instance;
            wallet.Initialize(context.Container.Resolve<SignalBus>());
            return wallet;
        }).AsSingle().NonLazy();

        Container.Bind<CharacterUpgradePrices>().FromInstance(_characterUpgradePrices).AsSingle().NonLazy();
        InstallAddressableGroupsComponents();
        InstallAddressableLoaders();
    }
    
    private void InstallAddressableGroupsComponents()
    {
        Container.BindInterfacesAndSelfTo<AddressableGroupsDataContainer>().FromInstance(_addressableDataContainer).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AddressableGroupsLoader>().AsSingle();
        Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();
        Container.BindInterfacesAndSelfTo<DataLoadingProgressTracker>().AsSingle();
    }

    private void InstallAddressableLoaders()
    {
        Container.Bind<MainMenuLoader>().AsSingle();
        Container.Bind<CompanyLoader>().AsSingle();
        Container.Bind<PvPLoader>().AsSingle();
        Container.Bind<TestOfStrengthLoader>().AsSingle();
        Container.Bind<ThreeToOneLoader>().AsSingle();
    }
}