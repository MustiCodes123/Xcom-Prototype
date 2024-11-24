using TMPro;
using UnityEngine;

namespace UI.CharacterWindow
{
    public class CharactersCounter : MonoBehaviour, ICounter
    {
        [SerializeField] private TextMeshProUGUI _charactersTMP;

        public void UpdateCounterView(int currentCount, int maxCount, string prefix = "")
        {
            _charactersTMP.text = $"{prefix}{currentCount}/{maxCount}";
        }
    }
}