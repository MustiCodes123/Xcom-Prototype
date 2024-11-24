using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScheduleItem : MonoBehaviour
{
    [field: SerializeField] public Image Image { get; set; }
    [field: SerializeField] public TMP_Text Amount { get; set; }
}