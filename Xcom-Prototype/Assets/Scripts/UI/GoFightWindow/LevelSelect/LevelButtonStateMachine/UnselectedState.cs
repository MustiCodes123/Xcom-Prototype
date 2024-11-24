public class UnselectedState : ILevelButtonViewState
{
    public void ApplyState(LevelButtonView button)
    {
        button.ChangeView(button.UnselectedConfig);
    }
}

public class UnselectedStageButtonState : IModeButtonViewState
{
    public void ApplyState(BattleModeButtonView button)
    {       
        button.ChangeView(button.UnselectedConfig);
    }
}

public class UnselectedShopCategoryButtonState : IShopCategoryButtonState
{
    public void ApplyState(ShopSelectableButton button)
    {
        button.ChangeView(button.UnselectedStateConfig);
    }
}

public class UnselectedChestButtonState : IChestButtonState
{
    public void ApplyState(ChestButtonView button)
    {
        button.ChangeView(button.UnselectedConfig);
    }
}