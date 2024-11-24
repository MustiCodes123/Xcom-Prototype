using System.Collections.Generic;
using UnityEngine;

public class OfferSwitcher : IOfferSwitcher
{
    private List<ISwitchableShopItem> _offers = new List<ISwitchableShopItem>();
    private ISwitchableShopItem _activeOffer;

    public void RegisterOffer(ISwitchableShopItem offer)
    {
        _offers.Add(offer);
    }

    public void UnregisterOffer(ISwitchableShopItem offer)
    {
        _offers.Remove(offer);
    }

    public void UnregisterAllOffers()
    {
        foreach(ISwitchableShopItem offer in _offers)
        {
            UnregisterOffer(offer);
        }
    }

    public void SwitchToOffer(ISwitchableShopItem offer)
    {
        var offersCopy = new List<ISwitchableShopItem>(_offers);

        foreach (var o in offersCopy)
        {
            if (o == offer)
            {
                o.SetActive(true);

                if (o.GetLinkedButton() != null)
                {
                    o.GetLinkedButton().SetState(new SelectedShopCategoryButtonState());
                }
            }
            else
            {
                o.SetActive(false);

                if (o.GetLinkedButton() != null)
                {
                    o.GetLinkedButton().SetState(new UnselectedShopCategoryButtonState());
                }
            }
        }

        _activeOffer = offer;
    }

    public void DeactivateAllOffers()
    {
        foreach (var offer in _offers)
        {
            offer.SetActive(false);
        }

        _activeOffer = null;
    }
}