using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonView : MonoBehaviour
{
    [SerializeField] private Image _slotImage;
    [SerializeField] private Sprite _selectedOutline;
    [SerializeField] private Sprite _unselectedOutline;
    [SerializeField] private TMP_Text _levelTMP;

    [field: SerializeField] public Image CharacterIcon { get; set; }

    public void Initialize(string avatarID, string level, PreBattleController controller)
    {
        CharacterIcon.sprite = controller.GetResourceManager().LoadSprite(avatarID);
        DisplayNotAssignedView();
        CharacterIcon.gameObject.SetActive(true);
        _levelTMP.text = level;
    }

    public void DisplayAssignedView()
    {
        _slotImage.sprite = _selectedOutline;
    }

    public void DisplayNotAssignedView()
    {
        _slotImage.sprite = _unselectedOutline;
    }
}