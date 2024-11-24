using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public enum DMItemType
{
    BaseItem,
    BaseCharacterModel,
    BaseSkillModel
};

public class DMController : MonoBehaviour
{
    private enum Prefixes
    {
        addItem_,
        addCharacter_,
        addSkill_
    };

    [Inject] private PlayerData _playerData;
    [SerializeField] private DMWindowView _view;
    [SerializeField] private TMP_InputField _searchField;
    [SerializeField] private DMAddItemButton _addItemButton;
    [SerializeField] private DMAddXPButton _addXPButton;
    [SerializeField] private DMItem _item;
    [SerializeField] private Toggle _infinityHealthToggle;
    [SerializeField] private Toggle _infinityManaToggle;

    private void OnEnable()
    {
        DeleteKeys();
        SetupEvents();
    }

    private void OnDisable()
    {
        SetupEvents(true);
    } 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _view.gameObject.SetActive(!_view.gameObject.activeSelf);
        }
    }

    private void SetupEvents(bool isRemoveListener = false)
    {
        if (!isRemoveListener)
        {
            _searchField.onEndEdit.AddListener(text => OnSearchFieldEndEdit(text));
            _addItemButton.Click += OnAddItemClick;
            _addXPButton.Click += OnAddXPClick;

            _infinityHealthToggle.onValueChanged.AddListener(value => SetInfiniteHealth(value));
            _infinityManaToggle.onValueChanged.AddListener(value => SetInfiniteMana(value));
        }
        else
        {
            _searchField.onEndEdit.RemoveListener(text => OnSearchFieldEndEdit(text));
            _addItemButton.Click -= OnAddItemClick;
            _addXPButton.Click -= OnAddXPClick;

            _infinityHealthToggle.onValueChanged.RemoveListener(value => SetInfiniteHealth(value));
            _infinityManaToggle.onValueChanged.RemoveListener(value => SetInfiniteMana(value));
        }
    }

    private void DeleteKeys()
    {
        PlayerPrefs.DeleteKey("ENABLE_INFINITY_HEALTH");
        PlayerPrefs.DeleteKey("ENABLE_INFINITY_MANA");
    }

    private void OnSearchFieldEndEdit(string inputText)
    {
        if (inputText.StartsWith(Prefixes.addCharacter_.ToString()))
        {
            string characterName = CutString(Prefixes.addCharacter_.ToString(), inputText);
            _view.InitializeItemView(characterName, DMItemType.BaseCharacterModel);
        }

        else if (inputText.StartsWith(Prefixes.addItem_.ToString()))
        {
            string itemName = CutString(Prefixes.addItem_.ToString(), inputText);
            _view.InitializeItemView(itemName, DMItemType.BaseItem);
        }

        else if (inputText.StartsWith(Prefixes.addSkill_.ToString()))
        {
            string skillName = CutString(Prefixes.addSkill_.ToString(), inputText);
            _view.InitializeItemView(skillName, DMItemType.BaseSkillModel);
        }
    }

    private void OnAddItemClick(IDMData data)
    {
        _item.AddToInventory(data);
    }

    private void OnAddXPClick(int value)
    {
        _playerData.PlayerXP += value;
        _view.ShowCurrentXP();
    }

    private void SetInfiniteHealth(bool isInfinite)
    {
        if (isInfinite)
        {
            PlayerPrefs.SetString("ENABLE_INFINITY_HEALTH", "someStr");

            Debug.Log(">>>ENABLE_INFINITY_HEALTH");
        }
        else
        {
            PlayerPrefs.DeleteKey("ENABLE_INFINITY_HEALTH");

            Debug.Log(">>>INFINITY_HEALTH_DISABLED");
        }
    }

    private void SetInfiniteMana(bool isInfinite)
    {
        if (isInfinite)
        {
            PlayerPrefs.SetString("ENABLE_INFINITY_MANA", "someStr");

            Debug.Log(">>>ENABLE_INFINITY_MANA");
        }
        else
        {
            PlayerPrefs.DeleteKey("ENABLE_INFINITY_MANA");

            Debug.Log(">>>INFINITY_MANA_DISABLED");
        }
    }

    private string CutString(string prefix, string convertString)
    {
        int prefixLength = prefix.Length;

        string readyString = convertString[prefixLength..];

        return readyString;
    }
}
