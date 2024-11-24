using Zenject;

public class SlotsExtensionStoragePopUp : SlotsExtensionPopUp
{
    [Inject] private PlayerData _playerData;

    protected override void OnGemsButtonClick()
    {
        if (ExtensionType == ExtensionType.Character)
        {
            if(_playerData.CharacterExtension < ExtensionSettings.CharacterSlotsExtensionLevels.Count)
            {
                bool isEnoughCurrency = Wallet.Instance.SpendCachedCurrency(GameCurrencies.Gem, (uint)ExtensionSettings.CharacterSlotsExtensionLevels[_playerData.CharacterExtension].GemPrice);

                if (isEnoughCurrency)
                {
                    _playerData.PlayerGroup.MaxGroupSize += ExtensionSettings.CharacterSlotsExtensionLevels[_playerData.CharacterExtension].SlotCount;
                    _playerData.CharacterExtension++;
                    gameObject.SetActive(false);
                }
            }
        }

        else if (ExtensionType == ExtensionType.Inventory)
        {
            if (_playerData.InventoryExtention < ExtensionSettings.InventorySlotsExtensionLevels.Count)
            {
                bool isEnoughCurrency = Wallet.Instance.SpendCachedCurrency(GameCurrencies.Gem, (uint)ExtensionSettings.InventorySlotsExtensionLevels[_playerData.InventoryExtention].GemPrice);

                if (isEnoughCurrency)
                {
                    _playerData.CurrentInventorySize += ExtensionSettings.InventorySlotsExtensionLevels[_playerData.InventoryExtention].SlotCount;
                    _playerData.InventoryExtention++;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    protected override void OnGoldButtonClick()
    {
        if (ExtensionType == ExtensionType.Character)
        {
            if (_playerData.CharacterExtension < ExtensionSettings.CharacterSlotsExtensionLevels.Count)
            {
                bool isEnoughCurrency = Wallet.Instance.SpendCachedCurrency(GameCurrencies.Gold, (uint)ExtensionSettings.CharacterSlotsExtensionLevels[_playerData.CharacterExtension].GoldPrice);

                if (isEnoughCurrency)
                {
                    _playerData.PlayerGroup.MaxGroupSize += ExtensionSettings.CharacterSlotsExtensionLevels[_playerData.CharacterExtension].SlotCount;
                    _playerData.CharacterExtension++;
                    gameObject.SetActive(false);
                }
            }
        }

        else if (ExtensionType == ExtensionType.Inventory)
        {
            if (_playerData.InventoryExtention < ExtensionSettings.InventorySlotsExtensionLevels.Count)
            {
                bool isEnoughCurrency = Wallet.Instance.SpendCachedCurrency(GameCurrencies.Gold, (uint)ExtensionSettings.InventorySlotsExtensionLevels[_playerData.InventoryExtention].GoldPrice);

                if (isEnoughCurrency)
                {
                    _playerData.CurrentInventorySize += ExtensionSettings.InventorySlotsExtensionLevels[_playerData.InventoryExtention].SlotCount;
                    _playerData.InventoryExtention++;
                    gameObject.SetActive(false);
                }
            }
        }

    }
}
