public interface ICommand
{
    public void Execute();
    public bool CanExecute();
    public int CalculateXP();
}
