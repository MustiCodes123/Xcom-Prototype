using System;
using UnityEngine;

[Serializable]
public class Mode
{
    public GameMode GameMode => _gameMode;
    public WindowsEnum Window => _window;
    public string Name => _name;
    public Sprite CardSprite => _cardSprite;

    [SerializeField] private GameMode _gameMode;
    [SerializeField] private WindowsEnum _window;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _cardSprite;
}

public enum GameMode
{
    Default,
    PvP,
    ThreeToOne,
    TestOfStrenght
}
