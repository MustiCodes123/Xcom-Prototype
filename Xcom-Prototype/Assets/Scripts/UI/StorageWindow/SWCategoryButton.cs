using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class SWCategoryButtonViewConfig
{
    [field: SerializeField] public TMP_FontAsset Font { get; private set; }
    [field: SerializeField] public float BackgroundAlpha { get; private set; }
}

[RequireComponent(typeof(Button))]
public class SWCategoryButton : MonoBehaviour
{
    public SWCategoryButtonViewConfig SelectedConfig;
    public SWCategoryButtonViewConfig UnselectedConfig;

    [SerializeField] private TMP_Text _text;
    [SerializeField] private Image _background;

    [field: SerializeField] public Button Button { get; private set; }

    private void OnEnable()
    {
        if(Button == null)
            Button = GetComponent<Button>();
    }

    public void ChangeView(SWCategoryButtonViewConfig config)
    {
        _text.font = config.Font;
        _background.color = new Color(1, 1, 1, config.BackgroundAlpha);
    }
}
