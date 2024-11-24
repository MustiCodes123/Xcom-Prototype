using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour
{
    public Image MainImage => _mainImage;
    public TMP_Text Description => _description;
    public Button Button => _button;

    [SerializeField] private Image _mainImage;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Button _button;
}
