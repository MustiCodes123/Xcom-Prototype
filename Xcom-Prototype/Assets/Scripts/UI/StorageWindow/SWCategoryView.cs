using UnityEngine;

public class SWCategoryView : MonoBehaviour
{
    [SerializeField] private GameObject _parent;

    [field: SerializeField] public Transform ItemsContainer { get; private set; }
    [field: SerializeField] public SWCategoryButton CategoryButton { get; private set; }

    public void Show()
    {
        _parent.SetActive(true);
        CategoryButton.ChangeView(CategoryButton.SelectedConfig);
    }

    public void Hide()
    {
        _parent.SetActive(false);
        CategoryButton.ChangeView(CategoryButton.UnselectedConfig);
    }
}