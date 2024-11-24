using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillCard : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static Action<bool> ItemDragStateChanged;

    public BaseSkillModel Skill;
    public TalentsEnum TalentType;
    public Action<BaseSkillModel> OnButtonClicked;
    public bool IsDragable;
    
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _skillName;
    [SerializeField] private TextMeshProUGUI _skillDescription;
    [SerializeField] private TextMeshProUGUI _skillLevel;
    [SerializeField] private Image _icon;
    [SerializeField] private Transform _iconParent;
    [SerializeField] private Transform _iconSlot;
    [SerializeField] private Transform _content;

    private PlayerData playerData;
    private BaseCharacterModel baseCharacterModel;
    private BaseSkillModel baseSkillModel;
    private Canvas _canvas;
    private SkillSelectPopup skillSelectPopup;
    private SkillSlot _mySlot;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClick);
        
        _canvas = GetComponentInParent<Canvas>();
    }

    public void Setup(BaseCharacterModel baseCharacterModel, SkillSelectPopup skillSelectPopup, PlayerData playerData, bool isDragable = true)
    {
        this.skillSelectPopup = skillSelectPopup;
        this.playerData = playerData;
        this.baseCharacterModel = baseCharacterModel; 
        _icon.sprite = emptySprite;
        IsDragable = isDragable;
    }

    public void FreeSlot()
    {
        _icon.sprite = emptySprite;
        _skillName.text = "";
        baseSkillModel  = null;
    }

    public void Lock()
    {
        _skillName.text = "LOCKED";
    }

    public void SetupSkill(BaseSkillModel baseSkillModel)
    {
        gameObject.SetActive(true);
        Skill = baseSkillModel;
        this.TalentType = baseSkillModel.Id;
        this.baseSkillModel = baseSkillModel;
        if (baseSkillModel.Level > 0)
        {
            _skillLevel.text = baseSkillModel.Level.ToString();
        }
        else
        {
            _skillLevel.text = "";
        }
        _icon.sprite = baseSkillModel.Icon;
        _skillName.text = baseSkillModel.Name;
        _skillLevel.text = baseSkillModel.Level.ToString();
        _skillDescription.text = baseSkillModel.Description;
    }


    private void OnButtonClick()
    {
        if (OnButtonClicked != null)
        {
            OnButtonClicked?.Invoke(baseSkillModel);
            return;
        }
        
        if (_mySlot != null)
        {
            SkillInfoPopUp.Instance.Show(Skill);
            return;
        }
        
        var availableSlot = FindAvailableSlot();
        if (availableSlot != null)
        {
            availableSlot.TryAssignSkill(this);
        }
    }

    private SkillSlot FindAvailableSlot()
    {
        var skillSlots = skillSelectPopup.GetAvalableSkillSlots();
        foreach (var slot in skillSlots)
        {
            if (!slot.IsFull)
            {
                return slot;
            }
        }
        return null;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsDragable)
        {
            return;
        }

        _iconSlot.transform.SetParent(_canvas.transform);
        SkillCard dragableItem = eventData.pointerDrag.GetComponent<SkillCard>();
        if (dragableItem != null)
        {
            _iconSlot.transform.SetAsLastSibling();
            _icon.raycastTarget = false;
        }

        ItemDragStateChanged?.Invoke(true);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDragable)
        {
            return;
        }

        _iconSlot.transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDragable)
        {
            return;
        }

        _icon.raycastTarget = true;

        if(Skill.isInUse)
        {
            _iconSlot.transform.SetParent(this.transform);
        }
        else
        {
            _iconSlot.transform.SetParent(_iconParent);
        }

        _iconSlot.localPosition = Vector3.zero;

        ItemDragStateChanged?.Invoke(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.Instance.ShowTooltip(baseSkillModel, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.HideTooltip();
    }

    public void SetInSlot(SkillSlot slot)
    {
        _mySlot = slot;
        _mySlot.RemoveButtonSetActive(true);
        _mySlot.RemoveSkillButton.onClick.AddListener(() => RemoveSkillFromSlot(Skill));
        SetInSlotState();
    }

    public void SetInSlotState()
    {
        _content.gameObject.SetActive(false);
        _iconSlot.gameObject.transform.SetParent(this.gameObject.transform);
        _iconSlot.gameObject.transform.localPosition = Vector3.zero;
        _iconSlot.gameObject.transform.localScale = Vector3.one;
    }

    public void SetInListState()
    {
        _content.gameObject.SetActive(true);
        _iconSlot.gameObject.transform.parent = _iconParent;
        _iconSlot.gameObject.transform.localPosition = Vector3.zero;
        IsDragable = true;
    }

    private void RemoveSkillFromSlot(BaseSkillModel skill)
    {
        skillSelectPopup.RemoveSkillByID(skill);
        skillSelectPopup.AddCard(this);
        transform.SetParent(skillSelectPopup.GetAvalableSkillsParent());
     
        if (_mySlot != null)
        {
            _mySlot.IsFull = false;
            _mySlot.RemoveButtonSetActive(false);
            
        }
        
        _mySlot = null;
        OnButtonClicked = null;
        SetInListState();
        
        if (Skill != null)
        {
            Skill.isInUse = false;
        }
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}
