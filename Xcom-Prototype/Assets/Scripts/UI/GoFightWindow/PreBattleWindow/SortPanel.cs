using System;
using UnityEngine;
using UnityEngine.UI;

public class SortPanel : MonoBehaviour
{
    [SerializeField] private Button _sortByLevelButton;
    [SerializeField] private Button _sortByNameButton;
    [SerializeField] private Button _sortByRarityButton;

    public void OnClose()
    {
        _sortByLevelButton.onClick.RemoveAllListeners();
        _sortByNameButton.onClick.RemoveAllListeners();
        _sortByRarityButton.onClick.RemoveAllListeners();
    }

    public void SublcribeOnLevelSortEvent(Action action)
    {
        _sortByLevelButton.onClick.AddListener(() => action?.Invoke());
    }

    public void SublcribeOnNameSortEvent(Action action)
    {
        _sortByNameButton.onClick.AddListener(() => action?.Invoke());
    }

    public void SublcribeOnRaritySortEvent(Action action)
    {
        _sortByRarityButton.onClick.AddListener(() => action?.Invoke());
    }
}
