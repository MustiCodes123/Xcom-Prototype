public class SelectedState : ILevelButtonViewState
{
    public void ApplyState(LevelButtonView button)
    {
        button.ChangeView(button.SelectedConfig);
    }
}

public class SelectedStageButtonState : IModeButtonViewState
{
    public void ApplyState(BattleModeButtonView button)
    {
        button.ChangeView(button.SelectedConfig);
    }
}

public class SelectedShopCategoryButtonState : IShopCategoryButtonState
{
    public void ApplyState(ShopSelectableButton button)
    {
        button.ChangeView(button.SelectedStateConfig);
    }
}

public class SelectedChestButtonState : IChestButtonState
{
    public void ApplyState(ChestButtonView button)
    {
        button.ChangeView(button.SelectedConfig);
    }
}