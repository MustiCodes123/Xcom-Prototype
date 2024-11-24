public interface IOfferSwitcher
{
    public void RegisterOffer(ISwitchableShopItem offer);
    public void SwitchToOffer(ISwitchableShopItem offer);
    public void DeactivateAllOffers();
}
