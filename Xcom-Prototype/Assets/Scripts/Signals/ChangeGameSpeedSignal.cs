namespace Signals
{
    public class ChangeGameSpeedSignal : ISignal
    {
        public readonly int NewSpeed;

        public ChangeGameSpeedSignal(int newSpeed)
        {
            NewSpeed = newSpeed;
        }
    }
}