using System.Threading.Tasks;
using Data.Resources.AddressableManagement;
using UnityEngine;

public class SpawnObjectOnDeath : MonoBehaviour
{
    private int xp_toDrop;
    private int gold_toDrop;
    private bool _isLastEnemy;

    private DropItemViewEnum _dropItemType;

    private PlayerData _playerData;
    private SaveManager _saveManager;
    private TemploaryInfo _temploaryInfo;
    private ResourceManager _resourceManager;
    private BaseCharacerView _myCharacterView;
    private ItemView.Factory _itemFactory;

    public void Setup(BaseCharacerView owner, TemploaryInfo temploaryInfo, ResourceManager resourceMnager = null, bool isLastEnemy = false, ItemView.Factory itemFactory = null)
    {
        _temploaryInfo = temploaryInfo;
        _resourceManager = resourceMnager;
        _myCharacterView = owner;
        _isLastEnemy = isLastEnemy;
        _itemFactory = itemFactory;
        CalculateXP();
        CalculateGold();
        _dropItemType = GetDropItemType(_isLastEnemy);
        _myCharacterView.OnDestroyAction += SpawnDrop;
    }

    public async void SpawnDrop()
    {
        switch (_dropItemType)
        {
            case DropItemViewEnum.DropItemGold:
                await SpawnGold();
                break;
            case DropItemViewEnum.DropItemXP:
                await SpawnXP();
                break;
            case DropItemViewEnum.DropItemWeapon:
                await SpawnWeapon();
                break;

            default:
                break;
        }
    }

    private void CalculateXP()
    {
        xp_toDrop = _temploaryInfo.LevelInfo.XP / _temploaryInfo.Score.GetEnemiesOnLevelCount();
    }
    private void CalculateGold()
    {
        gold_toDrop = _temploaryInfo.LevelInfo.Gold / _temploaryInfo.Score.GetEnemiesOnLevelCount();
    }

    private async Task SpawnGold()
    {
        var dropTask = _itemFactory.Create(_dropItemType.ToString());
        var dropObject = await dropTask;
        var dropView = dropObject as DropItemView;
        dropView.transform.localPosition = gameObject.transform.position;
        dropView.Setup(gold_toDrop, _dropItemType, Color.yellow);
    }

    private async Task SpawnXP()
    {
        var dropTask = _itemFactory.Create(_dropItemType.ToString());
        var dropObject = await dropTask;
        var dropView = dropObject as DropItemView;
        dropView.transform.localPosition = gameObject.transform.position;
        dropView.Setup(xp_toDrop, _dropItemType, Color.blue);
    }

    private async Task SpawnWeapon()
    {
        if (_temploaryInfo.CompanyItemRewards.Count > 0 && _temploaryInfo.CompanyItemRewards[0] is WeaponItem weapon && weapon != null)
        {
            var dropTask = _itemFactory.Create(_dropItemType.ToString());
            var dropObject = await dropTask;
            var dropView = dropObject as DropWeaponView;
            dropView.transform.localPosition = gameObject.transform.position;
            dropView.Setup(weapon, _dropItemType);

            var weaponTask = _itemFactory.Create(weapon.itemName);
            var weaponObject = await weaponTask;
            weaponObject.transform.SetParent(dropObject.transform);
            weaponObject.transform.localPosition = Vector3.zero;
            dropView.SetWeapon(weaponObject.gameObject);
        }
    }

    private DropItemViewEnum GetDropItemType(bool isLastEnemy)
    {
        if (isLastEnemy)
        {
            Debug.Log("LAST ENEMY!!!!!");
            return DropItemViewEnum.DropItemWeapon;
        }
        var random = new System.Random();
        int randomNumber = random.Next(0, 2);

        if (randomNumber == 0)
        {
            return DropItemViewEnum.DropItemXP;
        }
        else if (randomNumber == 1)
        {
            return DropItemViewEnum.DropItemGold;
        }

        return DropItemViewEnum.DropItemWeapon;
    }
}
