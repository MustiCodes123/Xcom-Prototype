using System;
using Data.Resources.AddressableManagement;
using UnityEngine;
using Zenject;

public class SelectedPlayerTeamView : MonoBehaviour
{
    [Inject] private ResourceManager _resourceManager;
    [field: SerializeField] public Transform[] CharacterDisplayPositions { get; private set; }

    #region Public Methods
    public void SetDisplayPositionsCount(GameMode gameMode)
    {
        Debug.Log($"SetDisplayPositionsCount {gameMode}");

        switch (gameMode)
        {
            case (GameMode.ThreeToOne):
                SetThreeToOnePositionsCount();
                break;

            default:
                SetDefaultPositionsCount();
                break;
        }
    }

    public void ShowSelectedCharacter(CharacterButton button)
    {
        PlaceholderID nextAvailableID = GetNextAvailablePlaceholderID();

        if (nextAvailableID == PlaceholderID.None)
            return;

        button.PlaceholderID = nextAvailableID;

        int positionIndex = (int)nextAvailableID;
        Transform displayPosition = CharacterDisplayPositions[positionIndex];

        GameObject characterPrefab = _resourceManager.LoadForUICharacter(button.CharacterModel.Name);

        if (characterPrefab != null)
        {
            GameObject characterInstance = Instantiate(characterPrefab, displayPosition);
            var itemHolder = characterInstance.GetComponent<CharacterSlotsHolder>();
            var equippedItems = button.CharacterModel.EquipedItems;
            itemHolder.SetupItems(equippedItems, _resourceManager);
        }
        else
        {
            Debug.LogError($"Character prefab '{button.CharacterModel.Name}' not found or position is occupied.");
        }
    }

    public void ClearCharacterDisplay(CharacterButton button)
    {
        if (button.PlaceholderID == PlaceholderID.None) return;

        int positionIndex = (int)button.PlaceholderID;
        Transform displayPosition = CharacterDisplayPositions[positionIndex];

        foreach (Transform child in displayPosition)
        {
            if (child.GetSiblingIndex() == 0)
                Destroy(child.gameObject);
        }

        button.PlaceholderID = PlaceholderID.None;
    }
    #endregion

    #region Utility Methods
    private PlaceholderID GetNextAvailablePlaceholderID()
    {
        for (int i = 0; i < CharacterDisplayPositions.Length; i++)
        {
            if (CharacterDisplayPositions[i].childCount == 0)
            {
                return (PlaceholderID)(i);
            }
        }

        return PlaceholderID.None;
    }

    private void SetDefaultPositionsCount()
    {
        Array.ForEach(CharacterDisplayPositions, position => position.gameObject.SetActive(true));
    }

    private void SetThreeToOnePositionsCount()
    {
        int lastElementIndex = CharacterDisplayPositions.Length - 1;
        int displayPositionsCount = GameConstants.ThreeToOneCharacterLimit;

        for (int i = lastElementIndex; i >= displayPositionsCount; i--)
        {
            CharacterDisplayPositions[i].gameObject.SetActive(false);
        }
    }
    #endregion
}