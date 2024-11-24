public class ModeCard : Card
{
    public Mode Mode => _mode;

    private Mode _mode;

    public void SetupCard(Mode mode)
    {
        _mode = mode;
        Description.text = _mode.Name;
        MainImage.sprite = _mode.CardSprite;
    }
}
