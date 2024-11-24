using UnityEngine;
using Zenject;

public class TrainingWindowInstaller : MonoInstaller
{
    [SerializeField] private TrainingCharacterCard _trainingCharacterCard;
    [SerializeField] private TrainingCharacterList _trainingCharacterList;
    [SerializeField] private ActivateUIInfo _activateUI;
    [SerializeField] private GameObject[] _locks;
    [SerializeField] private SmalCharacterCard[] _smalCharacterCards;
    [SerializeField] private CharacterUpgradePrices _characterUpgradePrices;

    public override void InstallBindings()
    {
        Container.Bind<TrainingCharacterCard>().FromInstance(_trainingCharacterCard).AsSingle();
        Container.Bind<TrainingCharacterList>().FromInstance(_trainingCharacterList).AsSingle();
        Container.Bind<ActivateUIInfo>().FromInstance(_activateUI).AsSingle();
        Container.Bind<GameObject[]>().WithId("locks").FromInstance(_locks).AsSingle();
        Container.Bind<SmalCharacterCard[]>().FromInstance(_smalCharacterCards).AsSingle();
        Container.Bind<CharacterUpgradePrices>().FromInstance(_characterUpgradePrices).AsSingle();
        Container.Bind<ICharacterUpgradeStrategy>().WithId("LevelUpStrategy").To<LevelUpStrategy>().AsTransient();
        Container.Bind<ICharacterUpgradeStrategy>().WithId("RankUpStrategy").To<RankUpStrategy>().AsTransient();
        Container.Bind<TrainingDataContainer>().AsSingle();
        Container.Bind<ITrainingView>().To<TrainingWindow>().FromComponentInHierarchy().AsSingle();
    }
}
