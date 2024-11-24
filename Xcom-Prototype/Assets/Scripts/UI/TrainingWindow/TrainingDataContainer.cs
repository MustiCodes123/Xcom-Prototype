using UnityEngine;
using Zenject;

public class TrainingDataContainer
{
    public TrainingCharacterList TrainingCharacterList { get; private set; }
    public TrainingCharacterCard TrainingCharacterCard { get; private set; }
    public SmalCharacterCard[] SmallCharacterCards { get; private set; }
    public CharacterUpgradePrices CharacterUpgradePrices { get; private set; }
    public SignalBus SignalBus { get; private set; }
    public GameObject[] Locks { get; private set; }

    [Inject]
    public TrainingDataContainer(
        TrainingCharacterList trainingCharacterList,
        TrainingCharacterCard trainingCharacterCard,
        SmalCharacterCard[] smalCharacterCards,
        CharacterUpgradePrices characterUpgradePrices,
        SignalBus signalBus,
        [Inject(Id = "locks")] GameObject[] locks)
    {
        TrainingCharacterList = trainingCharacterList;
        TrainingCharacterCard = trainingCharacterCard;
        SmallCharacterCards = smalCharacterCards;
        CharacterUpgradePrices = characterUpgradePrices;
        SignalBus = signalBus;
        Locks = locks;
    }

    public BaseCharacterModel CurrentCharacter { get; set; }
}