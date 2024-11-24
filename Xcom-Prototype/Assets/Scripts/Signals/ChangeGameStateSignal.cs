namespace Signals
{
    public class ChangeGameStateSignal : ISignal
    {
        public readonly GameState NewState;

        public ChangeGameStateSignal(GameState newState)
        {
            NewState = newState;
        }
    }


    public enum GameState
    {
        Gameplay,
        Pause,
    }
}