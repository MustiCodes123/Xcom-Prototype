using UnityEngine;
using UnityEngine.UI;

public class BuffIconContainer : MonoBehaviour
{
    public Image[] icons;
    public Image bigIcon;
    public IBuff buff;

    public void SetBuff(Sprite icon)
    {
        foreach (var iconSlot in icons)
        {
            if (iconSlot.sprite == null)
            {
                iconSlot.enabled = true;
                iconSlot.sprite = icon;
                break;
            }
            
        }
    }

    public void ReleveIcon(Sprite icon)
    {
        foreach (var iconSlot in icons)
        {
            if(iconSlot.sprite != null && iconSlot.sprite == icon)
            {
                iconSlot.sprite = null;
                iconSlot.enabled = false;
                break;
            }
        }
    }

    public void SetBigIcon(Sprite icon)
    {
        bigIcon.enabled = true;
        bigIcon.sprite = icon;
    }

    public void ReleaveBigIcon()
    {
        bigIcon.sprite = null;
        bigIcon.enabled = false;
    }
}