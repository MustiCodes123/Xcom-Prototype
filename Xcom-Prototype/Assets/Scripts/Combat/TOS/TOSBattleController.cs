using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TOSBattleController : MonoBehaviour
{
    [SerializeField] private UICombatCharacterHolder _uICombatCharacterHolder;
    [SerializeField] private BattleScene _battleScene;
    [SerializeField] private UIBattleWindow _battleWindow;
    [SerializeField] private CustomGameTimeScaleHandler _customGameTimeScaleHandler;
    [SerializeField] private CombatTimer _combatTimer;

    private CameraMovement _camera;

    private void Start()
    {
        _uICombatCharacterHolder.CreateCharactersCards();
        _uICombatCharacterHolder.CreatePvPCharactersCard();
        _battleWindow.Show();
        _combatTimer.Activate();

        _camera = Camera.main.GetComponent<CameraMovement>();
        _camera.Bounds = _battleScene.GetSceneBounds();

#if UNITY_EDITOR
        _camera.SetCameraState(new CameraEditorState(_customGameTimeScaleHandler));
#else
        _camera.SetCameraState(new CameraTouchState(_customGameTimeScaleHandler));
#endif
    }
}
