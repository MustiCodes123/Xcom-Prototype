using UnityEngine;
using Zenject;

public class FightWindowInstaller : MonoInstaller
{
    [SerializeField] private EnemiesIconsData _enemiesIconsData;
    [SerializeField] private BossIconsData _bossIconsData;
    [SerializeField] private LevelButton _levelButtonPrefab;
    [SerializeField] private StageSelectionButton _stageButtonPrefab;
    [SerializeField] private CharacterButton _characterButtonPrefab;
    [SerializeField] private RewardItemView _rewardIconPrefab;

    public override void InstallBindings()
    {
        Container.Bind<EnemiesIconsData>().FromInstance(_enemiesIconsData).AsSingle();
        Container.Bind<BossIconsData>().FromInstance(_bossIconsData).AsSingle();

        Container.Bind<FightWindowDataProvider>().AsSingle().NonLazy();

        Container.Bind<FightWindowPool>().AsSingle();

        Container.BindMemoryPool<LevelButton, UIObjectPool<LevelButton>>()
            .FromComponentInNewPrefab(_levelButtonPrefab);
        Container.BindMemoryPool<StageSelectionButton, UIObjectPool<StageSelectionButton>>()
            .FromComponentInNewPrefab(_stageButtonPrefab);
        Container.BindMemoryPool<CharacterButton, UIObjectPool<CharacterButton>>()
            .FromComponentInNewPrefab(_characterButtonPrefab);
        Container.BindMemoryPool<RewardItemView, UIObjectPool<RewardItemView>>()
            .FromComponentInNewPrefab(_rewardIconPrefab);
    }
}