using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingCharacterCard : MonoBehaviour
{
    public event Action<BaseCharacterModel> OnCharacterSelected;
    private Action OnCharacterRemoved;
    public event Action<bool> OnClosedCharCard;
    public bool HasCharacter => currentCharacter != null;

    [SerializeField] private GameObject empty;
    [SerializeField] private GameObject newCharacter;
    [SerializeField] private Image avatarImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private Button removeButton;

    private BaseCharacterModel currentCharacter;

    private void Awake()
    {
        removeButton.onClick.AddListener(RemoveButtonClick);
    }

    private void RemoveButtonClick()
    {
        currentCharacter = null;
        OnCharacterRemoved?.Invoke();
        OnClosedCharCard?.Invoke(false);
        OnCharacterSelected?.Invoke(null);
    }

    public void AddAction(Action action)
    {
        OnCharacterRemoved = action;
    }

    public void SetEmpty()
    {
        newCharacter.gameObject.SetActive(false);
        empty.gameObject.SetActive(true);
        currentCharacter = null;
    }

    public void SetData(BaseCharacterModel data)
    {
        newCharacter.gameObject.SetActive(true);
        empty.gameObject.SetActive(false);
        currentCharacter = data;
        avatarImage.sprite = Resources.Load<Sprite>("Avatars/" + data.AvatarId);
        nameText.text = data.Name;
        levelText.text = data.Level.ToString();

        UpdateStars(data);
        OnCharacterSelected?.Invoke(data);
    }

    public void UpdateStars(BaseCharacterModel data)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < data.Stars);
        }
    }

    public void OnDestroy()
    {
        removeButton.onClick.RemoveListener(RemoveButtonClick);
    }
}