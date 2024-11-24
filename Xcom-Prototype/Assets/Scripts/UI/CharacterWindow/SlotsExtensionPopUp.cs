using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotsExtensionPopUp : MonoBehaviour
{
    public Action<int, GameCurrencies> TryPurchaseInventorySlots;
    public Action<int, GameCurrencies> TryPurchaseCharactersSlots;

    protected ExtensionType ExtensionType;
    protected SlotsExtensionSettings ExtensionSettings;

    [SerializeField] private TMP_Text _goldPrice;
    [SerializeField] private TMP_Text _gemsPrice;

    [SerializeField] private Button _goldPurchaseButton;
    [SerializeField] private Button _gemsPurchaseButton;
    [SerializeField] private Button _closeButton;

    private InventoryExtensionLevel _targetLevel;

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        _goldPurchaseButton.onClick.AddListener(OnGoldButtonClick);
        _gemsPurchaseButton.onClick.AddListener(OnGemsButtonClick);
        _closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        _goldPurchaseButton.onClick.RemoveListener(OnGoldButtonClick);
        _gemsPurchaseButton.onClick.RemoveListener(OnGemsButtonClick);
        _closeButton.onClick.RemoveAllListeners();
    }
    #endregion

    #region View Methods
    public void Show(int extensionIndex, SlotsExtensionSettings slotsExtensionSettings, ExtensionType extensionType)
    {
        ExtensionType = extensionType;
        ExtensionSettings = slotsExtensionSettings;

        switch (extensionType)
        {
            case (ExtensionType.Character):
                if (extensionIndex < slotsExtensionSettings.CharacterSlotsExtensionLevels.Count)
                {
                    _targetLevel = slotsExtensionSettings.CharacterSlotsExtensionLevels[extensionIndex];
                    _goldPrice.text = _targetLevel.GoldPrice.ToString();
                    _gemsPrice.text = _targetLevel.GemPrice.ToString();
                    gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("No more character extensions available.");
                }
                break;

            case (ExtensionType.Inventory):
                if (extensionIndex < slotsExtensionSettings.InventorySlotsExtensionLevels.Count)
                {
                    _targetLevel = slotsExtensionSettings.InventorySlotsExtensionLevels[extensionIndex];
                    _goldPrice.text = _targetLevel.GoldPrice.ToString();
                    _gemsPrice.text = _targetLevel.GemPrice.ToString();
                    gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("No more inventory extensions available.");
                }
                break;
        }
    }
    #endregion

    #region Callbacks
    protected virtual void OnGoldButtonClick()
    {
        if (ExtensionType == ExtensionType.Character)
            TryPurchaseCharactersSlots?.Invoke(_targetLevel.GoldPrice, GameCurrencies.Gold);

        else if (ExtensionType == ExtensionType.Inventory)
            TryPurchaseInventorySlots?.Invoke(_targetLevel.GoldPrice, GameCurrencies.Gold);
    }

    protected virtual void OnGemsButtonClick()
    {
        if (ExtensionType == ExtensionType.Character)
            TryPurchaseCharactersSlots?.Invoke(_targetLevel.GemPrice, GameCurrencies.Gem);

        else if (ExtensionType == ExtensionType.Inventory)
            TryPurchaseInventorySlots?.Invoke(_targetLevel.GemPrice, GameCurrencies.Gem);
    }
    #endregion
}
