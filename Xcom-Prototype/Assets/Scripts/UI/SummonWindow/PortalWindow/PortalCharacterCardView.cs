using System;
using UnityEngine;

[RequireComponent(typeof(SmalCharacterCard))]
public class PortalCharacterCardView : MonoBehaviour
{
    [SerializeField] private SmalCharacterCard _characterCard;

    private void OnEnable()
    {
        if(_characterCard == null)
            _characterCard = GetComponent<SmalCharacterCard>();
    }

    public void ShowCard(CharacterPreset summonedCharacter, Action<BaseCharacterModel> clickAction)
    {
        _characterCard.SetCharacterData(summonedCharacter.CreateCharacter());
        _characterCard.SubscribeToClick(clickAction);

        gameObject.SetActive(true);
    }

    public void HideCard()
    {
        gameObject.SetActive(false);
    }

    public BaseCharacterModel GetCardData()
    {
        return _characterCard.baseCharacterInfo;
    }
}
