using UnityEngine;
using UnityEngine.UI;

public class BossSkillSlot : MonoBehaviour
{
    [SerializeField] private Image _image;

    public void Init(Sprite sprite) => _image.sprite = sprite;
}
