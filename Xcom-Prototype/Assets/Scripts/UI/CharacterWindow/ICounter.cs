namespace UI.CharacterWindow
{
    public interface ICounter
    {

        void UpdateCounterView(int currentCount, int maxCount, string prefix = "");
    }
}