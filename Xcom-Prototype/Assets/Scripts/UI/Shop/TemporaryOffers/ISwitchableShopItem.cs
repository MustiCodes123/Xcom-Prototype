using UnityEngine;

public interface ISwitchableShopItem
{
    public void CreateLinkedButton(Transform parent);

    public void SetActive(bool isActive);
    public TemporaryOfferCategoryButton GetLinkedButton();
}
