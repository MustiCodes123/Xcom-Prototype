using Data.Resources.AddressableManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterBattleSlot : MonoBehaviour
{
    [SerializeField] private Image[] _stars;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Image _characterImage;

    private ResourceManager _resourceManager;

    public void Initialize(int starCount, string levelText, Sprite characterSprite)
    {
        for (int i = 0; i < starCount; i++)
        {
            if (i < _stars.Length)
                _stars[i].gameObject.SetActive(true);
        }

        _levelText.text = levelText;
        _characterImage.sprite = characterSprite;
    }

    public void Initialize(BaseCharacterModel model, ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;

        for (int i = 0; i < model.Stars; i++)
        {
            _stars[i].gameObject.SetActive(true);
        }

        _levelText.text = model.Level.ToString();
        _characterImage.sprite = _resourceManager.LoadSprite(model.AvatarId);
    }
}
