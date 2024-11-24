using UniRx;
using Zenject;

public class UniRxDisposable
{
    public CompositeDisposable MainMenuTimerDisposable;
    public CompositeDisposable UICharacterTimerDisposable;
    public CompositeDisposable CombatCharacterTimerDisposable;
    public CompositeDisposable SkillDelayTimerDisposable;
    public CompositeDisposable OpenWindowTimerDisposable;

    [Inject]
    public UniRxDisposable()
    {
        MainMenuTimerDisposable = new CompositeDisposable();
        UICharacterTimerDisposable = new CompositeDisposable();
        CombatCharacterTimerDisposable = new CompositeDisposable();
        SkillDelayTimerDisposable = new CompositeDisposable();
        OpenWindowTimerDisposable = new CompositeDisposable();
    }
}
