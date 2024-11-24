using UnityEngine;
using Zenject;

public class MainSceneInstaller : MonoInstaller
{
    [SerializeField] private BaseItemSlot  baseItemSlot;
    [SerializeField] private BaseDragableItem  baseDragableItem;
    [SerializeField] private UIWindowManager uIWindowManager;
    [SerializeField] private Tooltip Tooltip;
    [SerializeField] private InfoPopup InfoPopup;
    [SerializeField] private ItemInfoPopup ItemInfoPopup;
    [SerializeField] private SkillSelectPopup skillSelectPopup;
    [SerializeField] private CameraHolder cameraHolder;
    [SerializeField] private PlayerInventoryView inventoryPrefab;
    [SerializeField] private UICharacterVIew _uiCharacterVIew;
    [SerializeField] private SkillInfoPopUp _skillInfoPopUp;

    public override void InstallBindings()
    {
        Container.Bind<UIWindowManager>().FromInstance(uIWindowManager).AsSingle();
        Container.Bind<CameraHolder>().FromInstance(cameraHolder).AsSingle();
        Container.Bind<SkillSelectPopup>().FromInstance(skillSelectPopup).AsSingle();
        Container.Bind<PlayerInventoryView>().FromInstance(inventoryPrefab).AsSingle();
        Container.Bind<BaseItemSlot>().FromInstance(baseItemSlot).AsSingle();
        Container.Bind<BaseDragableItem>().FromInstance(baseDragableItem).AsSingle();
        Container.Bind<UIBottomPanel>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Tooltip>().FromInstance(Tooltip).AsSingle();
        Container.Bind<InfoPopup>().FromInstance(InfoPopup).AsSingle();
        Container.Bind<ItemInfoPopup>().FromInstance(ItemInfoPopup).AsSingle();
        Container.Bind<UICharacterVIew>().FromInstance(_uiCharacterVIew).AsSingle();
        Container.Bind<SkillInfoPopUp>().FromInstance(_skillInfoPopUp).AsSingle();
    }
}