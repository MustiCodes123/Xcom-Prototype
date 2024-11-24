using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using Zenject;

public class TeamMemberContent : MonoBehaviour
{
    [field: SerializeField] private Image _leaderIcon;
    [field: SerializeField] private TMP_Text _nameText;
    [field: SerializeField] private TMP_Text _levelText;
    [field: SerializeField] private TMP_Text _keysCount;
    [field: SerializeField] private Transform _charactersContainer;
    [field: SerializeField] private CharacterBattleSlot _characterSlotPrefab;

    private List<CharacterBattleSlot> _battleSlots = new List<CharacterBattleSlot>();

    [Inject] private ResourceManager _resourceManager;

    public void Init(BossData data, FakeLeader fakeLeader, int maxKeysCount)
    {
        ClearCharacterSlots();

        _leaderIcon.sprite = fakeLeader.Icon;
        _nameText.text = fakeLeader.Name;
        _levelText.text = fakeLeader.LeaderData.Level.ToString();
        _keysCount.text = Random.Range(data.KeysCost + 1, maxKeysCount).ToString() + " / " + maxKeysCount.ToString();

        for (int i = 0; i < GameConstants.ThreeToOneCharacterLimit; i++)
        {
            var characterSlot = Instantiate(_characterSlotPrefab, _charactersContainer);
            _battleSlots.Add(characterSlot);
            characterSlot.Initialize(fakeLeader.Characters[i].CharacterPreset.Stars, fakeLeader.CurrentSaveData.Level.ToString(), fakeLeader.Characters[i].CharacterPreset.CharacterSprite);
        }
    }

    public void InitPlayer(PlayerData data)
    {
        _leaderIcon.sprite = _resourceManager.LoadSprite(data.PlayerIconPath);
        _nameText.text = data.PlayerName;
        _levelText.text = data.PlayerLevel.ToString();
    }

    private void ClearCharacterSlots()
    {
        _battleSlots.ForEach(slot => Destroy(slot.gameObject));
        _battleSlots.Clear();
    }
}
