using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using UnityEngine;
using Zenject;

public class UICharacterVIew : MonoBehaviour
{
    [SerializeField] private Transform _characterParent;
    [SerializeField] private Transform _summoncharacterParent;
    [SerializeField] private Transform _backgroundParent;

    [Inject] private SignalBus signalBus;
    [Inject] private DiContainer _container;
    [Inject] private UniRxDisposable _uniRxDisposable;
    [Inject] private ResourceManager _resourceManager;

    private Dictionary<string, CharacterSlotsHolder> _spawnedCharacters = new Dictionary<string, CharacterSlotsHolder>();
    private CharacterSlotsHolder _currentCharacters;

    private Dictionary<string, ForUICharacterController> _summonedCharacters = new Dictionary<string, ForUICharacterController>();

    private ForUICharacterController _currentSummonedCharacter;

    private void Start()
    {
        signalBus.Subscribe<CharacterSelectSignal>(ShowCharacter);
        signalBus.Subscribe<CharacterSelectSignal>(ShowCharacterBackground);
        signalBus.Subscribe<CharacterEquipSignal>(CharacterEquiped);
        signalBus.Subscribe<SummonHeroSignal>(ShowSummonedCharacter);
        signalBus.Subscribe<SummonHeroSignal>(ShowSummonedCharacterBackground);
    }
    
    public void ResetCharacterParentRotation() => _characterParent.localRotation = Quaternion.identity;

    private void CharacterEquiped(CharacterEquipSignal signal)
    {
        if(_currentCharacters != null)
        {
            _currentCharacters.SetupItems(signal.baseCharacterInfo.EquipedItems, _resourceManager);
        }
    }

    private void CharacterEquiped(List<BaseItem> equipedItems)
    {
        if(_currentCharacters != null)
        {
            _currentCharacters.SetupItems(equipedItems, _resourceManager);
        }
    }

    private void ShowCharacter(CharacterSelectSignal signal)
    {
        ResetField();

        if (_spawnedCharacters.ContainsKey(signal.CharacterInfo.Name))
        {
            _currentCharacters = _spawnedCharacters[signal.CharacterInfo.Name];
            _currentCharacters.gameObject.SetActive(true);
        }
        else
        {
            CharacterSlotsHolder prefab = _resourceManager.LoadForUISlotHolder(signal.CharacterInfo.Name); 
            _currentCharacters = Instantiate<CharacterSlotsHolder>(prefab, _characterParent);
            _currentCharacters.SetupItems(signal.CharacterInfo.EquipedItems, _resourceManager);
            _spawnedCharacters.Add(signal.CharacterInfo.Name, _currentCharacters);
        }


        CharacterEquiped(signal.CharacterInfo.EquipedItems);
    }

    private void ShowSummonedCharacter(SummonHeroSignal signal)
    {
        ResetField();

        if (_summonedCharacters.ContainsKey(signal.Hero.PresetName))
        {
            _currentSummonedCharacter = _summonedCharacters[signal.Hero.PresetName];
            _currentSummonedCharacter.gameObject.SetActive(true);
            _currentSummonedCharacter.transform.localPosition = Vector3.zero;
            _currentSummonedCharacter.MoveCharacterWithDelay();
        }
        else
        {
            var prefab = _resourceManager.LoadForUIController(signal.Hero.PresetName);
            _currentSummonedCharacter = Instantiate<ForUICharacterController>(prefab, _summoncharacterParent);
            _currentSummonedCharacter.Init(_uniRxDisposable);
            _currentSummonedCharacter.transform.localPosition = Vector3.zero;
            _summonedCharacters.Add(signal.Hero.PresetName, _currentSummonedCharacter);
            _currentSummonedCharacter.MoveCharacterWithDelay();
        }
    }

    private void ShowCharacterBackground(CharacterSelectSignal signal)
    {
        DestroyInvalidBG();

        GameObject prefab = _resourceManager.LoadForUIBackground(signal.CharacterInfo.BackGroundPath);
        var background = _container.InstantiatePrefab(prefab, _backgroundParent);
        background.SetActive(true);
    }

    private void ShowSummonedCharacterBackground(SummonHeroSignal signal)
    {
        DestroyInvalidBG();

        GameObject prefab = _resourceManager.LoadForUIBackground(signal.Hero.BackGroundPath);
        var background = _container.InstantiatePrefab(prefab, _backgroundParent);
        background.SetActive(true);
    }

    public void ResetField()
    {
        foreach (var item in _summonedCharacters)
        {
            item.Value.gameObject.SetActive(false);
        }
        foreach (var item in _spawnedCharacters)
        {
            item.Value.gameObject.SetActive(false);
        }
    }

    public void DestroyInvalidBG()
    {
        foreach (Transform child in _backgroundParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
