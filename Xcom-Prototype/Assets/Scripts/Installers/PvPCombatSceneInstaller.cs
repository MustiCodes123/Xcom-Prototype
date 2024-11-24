using UnityEngine;

public class PvPCombatSceneInstaller : CombatSceneInstaller
{
    [SerializeField] private PvPCombatSpawner _pvpSpawner;

    public override void InstallBindings()
    {
        BattleController = null;
        CombatSpawner = null;
        base.InstallBindings();
    }
}
