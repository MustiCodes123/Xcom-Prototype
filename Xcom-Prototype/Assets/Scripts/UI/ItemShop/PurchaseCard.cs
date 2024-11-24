using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseCard : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Price;
    public Image Image;
    public Image PriceImage;
    public Button Button;

    private PurchaseItem purchaseItem;
    private Action<PurchaseItem> OnItemBought;

    public void Start()
    {
        Button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        Debug.Log("OnButtonClick");
        OnItemBought?.Invoke(purchaseItem);
    }

    public void SetItem(PurchaseItem item, Action<PurchaseItem> onItemBought)
    {
        Name.text = item.ItemName;
        Price.text = item.ItemPrice.ToString();
        Image.sprite = item.Sprite;
        OnItemBought = onItemBought;
        purchaseItem = item;

        PriceImage.gameObject.SetActive(false);
    }
}
