using TMPro;
using UnityEngine;

namespace UI.CharacterWindow
{
    public class WeaponCounter : MonoBehaviour, ICounter
    {
        [SerializeField] private TextMeshProUGUI _weaponTMP; 

        public void UpdateCounterView(int currentCount, int maxCount, string prefix = "")
        {
            _weaponTMP.text = $"{prefix}{currentCount}/{maxCount}";
        }
    }
}