using UnityEngine;
using TMPro;
using Zenject;

public enum LevelDifficulty
{
    Normal,
    Devilish,
    Impossible,
    Hard
};

[RequireComponent(typeof(TMP_Dropdown))]
public class DifficultyDropdown : MonoBehaviour
{
    [Inject] private FightWindowDataProvider _dataProvider;

    [SerializeField] private TMP_Dropdown _dropdown;

    private Difficulty _difficulty = Difficulty.Easy;

    private void OnEnable()
    {
        if(_dropdown == null)
            _dropdown = GetComponent<TMP_Dropdown>();

        _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDisable()
    {
        _dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int value)
    {
        _difficulty = (Difficulty)value;
        _dataProvider.CurrentDifficulty = _difficulty;
    }
}
