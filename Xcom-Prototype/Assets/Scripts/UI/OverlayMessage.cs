using TMPro;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class OverlayMessage : MonoBehaviour
{
    [Inject] private PlayerData _playerData;

    [SerializeField] private TMP_Text _messageTMP;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _displayDuration = 2f;

    private void Awake()
    {
        _playerData.PlayerGroup.CharacterMovedToStorage += Show;
        _playerData.ItemAddedToStorage += Show;

        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _playerData.PlayerGroup.CharacterMovedToStorage -= Show;
        _playerData.ItemAddedToStorage -= Show;
    }

    private void Show(BaseCharacterModel character)
    {
        _messageTMP.text = $"Not enough characters slots. Character {character.Name} was moved to storage";
        _canvasGroup.alpha = 1f;
        gameObject.SetActive(true);

        _canvasGroup.DOFade(0f, _fadeDuration)
            .SetDelay(_displayDuration)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    private void Show(BaseItem item)
    {
        _messageTMP.text = $"Not enough items slots. Item {item.itemName} was moved to storage";
        _canvasGroup.alpha = 1f;
        gameObject.SetActive(true);

        _canvasGroup.DOFade(0f, _fadeDuration)
            .SetDelay(_displayDuration)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}