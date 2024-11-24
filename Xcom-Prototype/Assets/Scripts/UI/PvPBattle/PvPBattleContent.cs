using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PvPBattleContent : MonoBehaviour
{
    [SerializeField] private TMP_Text _leaderNameText;
    [SerializeField] private TMP_Text _leaderLevelText;
    [SerializeField] private Image _leaderIconImage;

    [SerializeField] private Transform _charactersSlotParent;
    [SerializeField] private CharacterBattleSlot _battleSlotPrefab;

    [SerializeField] private TMP_Text _moneyCountText;
    [SerializeField] private Button _startBattleButton;

    private const int CharLevelValue = 3;

    [field: SerializeField] public Button ShowRewardsButton { get; private set; }

    public void Initialize(FakeLeader leaderData, Action<FakeLeader> action, PvPBattleData battleData)
    {
        _leaderNameText.text = leaderData.Name;
        _leaderLevelText.text = leaderData.CurrentSaveData.Level.ToString();
        _leaderIconImage.sprite = leaderData.Icon;

        for (int i = 0; i < leaderData.Characters.Length; i++)
        {
            var battleSlot = Instantiate(_battleSlotPrefab, _charactersSlotParent);
            var character = leaderData.Characters[i].CharacterPreset;
            var characterLevel = leaderData.CurrentSaveData.Level - UnityEngine.Random.Range(0, CharLevelValue);
            characterLevel = Mathf.Clamp(characterLevel, 0, leaderData.CurrentSaveData.Level);
            battleSlot.Initialize(character.Stars, characterLevel.ToString(), character.CharacterSprite);          
        }

        _moneyCountText.text = battleData.KeysCost.ToString();

        _startBattleButton.onClick.AddListener(() =>
        {
            action?.Invoke(leaderData);
        });
    }
}
