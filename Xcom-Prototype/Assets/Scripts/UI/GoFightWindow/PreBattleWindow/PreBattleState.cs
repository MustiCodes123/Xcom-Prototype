public abstract class PreBattleState
{
    protected PreBattleController _controller;

    public PreBattleState(PreBattleController controller)
    {
        _controller = controller;
    }

    public abstract void Initialize();
    public abstract void CreateOrUpdateCharacterButtons();
    public abstract void OnClose();
    public abstract void OnStartButtonClicked();
    public abstract void OnCharacterButtonClick(CharacterButton button);
    public abstract void OnSortByNameButtonClick();
    public abstract void OnSortByRarityButtonClick();
    public abstract void OnSortByLevelButtonClick();
}
