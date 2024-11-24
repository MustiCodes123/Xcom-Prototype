using System;
using Zenject;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PlaceholderID
{   
    First,
    Second,
    Third,
    Fourth,
    Fifth,
    None
};

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(CharacterButtonView))]
public class CharacterButton : MonoBehaviour
{
    [SerializeField] private Button _button;

    [field: SerializeField] public CharacterButtonView ButtonView { get; private set; }
    [field: SerializeField] public bool IsAssigned { get; set; }
    public BaseCharacterModel CharacterModel { get; set; }
    public PlaceholderID PlaceholderID { get; set; }

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        ButtonView = GetComponent<CharacterButtonView>();
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void Initialize(string avatarID, PreBattleController controller)
    {
        this.PlaceholderID = PlaceholderID.None;
        IsAssigned = false;

        ButtonView.Initialize(avatarID, CharacterModel.Level.ToString(), controller);
    }

    public void Subscribe(Action<CharacterButton> onCharacterChanged)
    {
        _button.onClick.AddListener(() => onCharacterChanged?.Invoke(this));
    }

    public void Unsubscribe()
    {
        _button.onClick.RemoveAllListeners();
    }
}