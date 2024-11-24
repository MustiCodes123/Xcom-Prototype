using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CharacterUIXPBar : MonoBehaviour
{
    [SerializeField] Image xpBar;
    [SerializeField] TextMeshProUGUI xpText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI ArrowLevelText;
    [SerializeField] TextMeshProUGUI ArrowLevelNextText;

    [Inject] private SignalBus signalBus;

    private const string MaxLevelMessage = "Max Level";

    private void Awake()
    {
        signalBus.Subscribe<CharacterSelectSignal>(OnCharacterChanged);
    }

    private void OnCharacterChanged(CharacterSelectSignal characterSelect)
    {
        SetupBar(characterSelect.CharacterInfo);
    }

    public void SetupBar(BaseCharacterModel baseCharacter, BaseCharacterModel? newCharacter=null)
    {
        if (baseCharacter.IsMaxLevel)
            SetupMaxLevelUI(baseCharacter);
        else
            SetupNormalLevelUI(baseCharacter, newCharacter);
    }
    
    private void SetupMaxLevelUI(BaseCharacterModel baseCharacter)
    {
        xpBar.fillAmount = 1;
        xpText.text = MaxLevelMessage;
        levelText.text = $"Max level";

        if (ArrowLevelText != null && ArrowLevelNextText != null)
        {
            ArrowLevelText.text = baseCharacter.Rare.ToString();
            if (!baseCharacter.IsMaxRank)
                ArrowLevelNextText.text = (baseCharacter.Rare + 1).ToString();
            else 
                ArrowLevelNextText.text = baseCharacter.Rare.ToString();
        }
    }

    private void SetupNormalLevelUI(BaseCharacterModel baseCharacter, BaseCharacterModel? newCharacter=null)
    {
        newCharacter ??= baseCharacter;
        
        xpBar.fillAmount = (float)newCharacter.Xp / newCharacter.XpToNextLevel;
        xpText.text = $"XP: {newCharacter.Xp}/{newCharacter.XpToNextLevel}";
        levelText.text = $"Level: {newCharacter.Level}/{newCharacter.Stars * 10}";

        if (ArrowLevelText != null && ArrowLevelNextText != null)
        {
            ArrowLevelText.text = "LVL " + baseCharacter.Level.ToString("D2");
            ArrowLevelNextText.text = "LVL " + newCharacter.Level.ToString("D2");
        }
    }

    public void ClearBar()
    {
        xpBar.fillAmount = 0;
        xpText.text = string.Empty;
        levelText.text = string.Empty;

        if (ArrowLevelText != null && ArrowLevelNextText != null)
        {
            ArrowLevelText.text = string.Empty;
            ArrowLevelNextText.text = string.Empty;
        }
    }
}
