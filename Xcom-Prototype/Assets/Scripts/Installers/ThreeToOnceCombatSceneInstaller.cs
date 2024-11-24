using UnityEngine;

public class ThreeToOnceCombatSceneInstaller : CombatSceneInstaller
{
    public override void InstallBindings()
    {
        BattleController = null;
        CombatSpawner = null;
        base.InstallBindings();
    }
}