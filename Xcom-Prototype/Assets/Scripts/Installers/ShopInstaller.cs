using Zenject;
using UnityEngine;

public class ShopInstaller : MonoInstaller
{
    [SerializeField] private ShopModel _model;
    [SerializeField] private ShopView _view;
    [SerializeField] private PurchaseConfirmPopUp _purchaseConfirmPopUp;
    [SerializeField] private NotEnoughPopUp _notEnoughPopUp;

    public override void InstallBindings()
    {
        Container.Bind<OfferSwitcher>().AsSingle();
        Container.Bind<ShopModel>().FromInstance(_model).AsSingle();
        Container.Bind<ShopView>().FromInstance(_view).AsSingle();
        Container.Bind<PurchaseConfirmPopUp>().FromInstance(_purchaseConfirmPopUp).AsSingle();
        Container.Bind<NotEnoughPopUp>().FromInstance(_notEnoughPopUp).AsSingle();
        Container.Bind<ShopPresenter>().ToSelf().AsSingle();
    }
}