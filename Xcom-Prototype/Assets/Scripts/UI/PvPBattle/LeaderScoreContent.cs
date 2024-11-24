using TMPro;
using UnityEngine;

public class LeaderScoreContent : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _placeText;
    [SerializeField] private TMP_Text _scoreText;

    public void Initialize(string name, string score, string place)
    {
        _nameText.text = name;
        _placeText.text = place;
        _scoreText.text = score;
    }
}
