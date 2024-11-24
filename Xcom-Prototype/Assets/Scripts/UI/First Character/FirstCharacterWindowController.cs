using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FirstCharacterWindowController : UIWindowView
{
    [Inject] private PlayerData _playerData;

    [SerializeField] private Button _selectCharacterButton;
    [SerializeField] private FirstCharactersList _firstCharactersList;
    [SerializeField] private Transform[] _characterPositions;
    [SerializeField] private PositionAndRotationData[] _characterOrientationData;
    [SerializeField] private FirstCharacterButton[] _characterButtons;
    [SerializeField] private Material _outlineMaterial;

    [SerializeField] private CharacterStatsViewer _characterStatsViewer;
    [SerializeField] private TMP_Text _currentCharacterName;
    [SerializeField] private GameObject[] _stars;

    private BaseCharacterModel _currentCharacter;
    private string _outlineMaterialName =>  $"{_outlineMaterial.name} (Instance)";

    private void OnEnable()
    {
        CreateCharacters();

        foreach (FirstCharacterButton button in _characterButtons)
        {
            button.Initialize();
            button.Click += OnCharacterClick;
        }

        _selectCharacterButton.onClick.AddListener(OnSelectCharacterButtonClick);

        SetSelected(0); // 0 character selected by default
    }

    private void OnDisable()
    {
        foreach (FirstCharacterButton button in _characterButtons)
        {
            button.Click -= OnCharacterClick;
        }

        _selectCharacterButton.onClick.RemoveListener(OnSelectCharacterButtonClick);
    }

    private void CreateCharacters()
    {
        if (_firstCharactersList.FirstCharacters.Length != _characterPositions.Length)
        {
            Debug.LogError($"Array length mismatch: FirstCharacters ({_firstCharactersList.FirstCharacters.Length}) != CharacterPositions ({_characterPositions.Length}). These arrays must have equal length.");
            return;
        }

        for (int i = 0; i < _firstCharactersList.FirstCharacters.Length; i++)
        {
            GameObject characterPrefab = _firstCharactersList.FirstCharacters[i].gameObject;

            GameObject instantiatedCharacter = Instantiate(characterPrefab, _characterPositions[i]);
            instantiatedCharacter.transform.SetLocalPositionAndRotation(_characterOrientationData[i].Position, Quaternion.Euler(_characterOrientationData[i].EulerAngles));
            _characterButtons[i].CharacterGameObject = instantiatedCharacter;
        }
    }

    private void OnCharacterClick(SkinnedMeshRenderer[] meshRenderers, BaseCharacterModel character)
    {
        HideOutlines();

        RenderOutline(meshRenderers);

        DisplayUIInfo(character);

        _currentCharacter = character;
    }

    private void RenderOutline(SkinnedMeshRenderer[] meshRenderers)
    {
        foreach(var meshRenderer in meshRenderers)
        {
            Material[] currentMaterials = meshRenderer.materials;
            Material[] newMaterials = new Material[currentMaterials.Length + 1];

            for (int i = 0; i < currentMaterials.Length; i++)
            {
                newMaterials[i] = currentMaterials[i];
            }

            newMaterials[newMaterials.Length - 1] = _outlineMaterial;

            meshRenderer.materials = newMaterials;
        }
    }

    private void HideOutlines()
    {
        foreach (var button in _characterButtons)
        {
            foreach(var meshRenderer in button.MeshRenderers)
            {
                Material[] characterModelMaterials = meshRenderer.materials;
                List<Material> newMaterials = new List<Material>();
                for (int i = 0; i < characterModelMaterials.Length; i++)
                {
                    if (characterModelMaterials[i].name != _outlineMaterialName)
                    {
                        newMaterials.Add(characterModelMaterials[i]);
                    }
                }

                meshRenderer.materials = newMaterials.ToArray();
            }
        }
    }

    private void DisplayUIInfo(BaseCharacterModel character)
    {
        _characterStatsViewer.SetCharacterStats(character);

        _currentCharacterName.text = $"{character.Name}";

        foreach (GameObject star in _stars)
        {
            star.SetActive(false);
        }

        for (int i = 0; i < character.Stars; i++)
        {
            _stars[i].SetActive(true);
        }
    }

    private void OnSelectCharacterButtonClick()
    {
        _playerData.PlayerGroup.AddCharacterToNotAsignedGroup(_currentCharacter);
        OnCloseClicked();
        gameObject.SetActive(false);
    }

    private void SetSelected(int index)
    {
        OnCharacterClick(_characterButtons[index].MeshRenderers, _characterButtons[index].CharacterModel);
    }
}