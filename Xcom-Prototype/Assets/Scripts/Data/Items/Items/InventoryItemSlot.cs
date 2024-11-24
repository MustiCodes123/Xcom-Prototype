using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class InventoryItemSlot : BaseItemSlot
{
    [SerializeField] private Button button;

    protected Image backgroundImage;
    private Image overlayImage;

    protected CharacterHandler characterSelectHandler;
    private SignalBus signalBus; 
    private PlayerInventoryView playerInventoryView;    
    private BaseCharacterModel currentBaseCharacterInfo;

    [Inject] 
    public void Construct(CharacterHandler characterSelectHandler, SignalBus signalBus, PlayerInventoryView playerInventoryView)
    {
        this.playerInventoryView = playerInventoryView;
        this.characterSelectHandler = characterSelectHandler;
        this.signalBus = signalBus;
    }

    private void Awake()
    {
        backgroundImage = transform.GetChild(0).GetComponent<Image>();

        if(button == null)
        {
            button = GetComponent<Button>();
        }

        button.onClick.AddListener(OnButtonClick);

        backgroundImage.gameObject.SetActive(Item == null);
    }

    private void OnButtonClick()
    {
        playerInventoryView.Show();
    }

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
    }

    private void Start()
    {
        signalBus.Subscribe<CharacterSelectSignal>(OnCharacterChanged);
    }

    private void OnCharacterChanged(CharacterSelectSignal baseCharacterSignal)
    {
        currentBaseCharacterInfo = baseCharacterSignal.CharacterInfo;
    }

    public override void UpdateInventory()
    {
        playerInventoryView.CreateInventory();
    }

    public override void SetItem(BaseItem item, bool needCreateNew = true, Action<BaseItem, BaseItemSlot> action = null)
    {
        base.SetItem(item, needCreateNew, action);
        backgroundImage?.gameObject.SetActive(false);
    }

    override public void Reset()
    {
        base.Reset();
        backgroundImage?.gameObject.SetActive(true);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        backgroundImage?.gameObject.SetActive(true);
    }

    public override void AfterDrop(BaseItemSlot Slotfrom, BaseItemSlot SlotTo)
    {
        base.AfterDrop(Slotfrom, SlotTo);
        characterSelectHandler.EquipItem(Item);
    }
}

