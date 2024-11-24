using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BattleController : MonoBehaviour
{
    public Action OnMoveToCombatZone;
    public Action OnCompleteMoveToCombatZone;
    public Action OnStartBattle;

    [SerializeField] private UICombatCharacterHolder _uICombatCharacterHolder;
    [SerializeField] private BattleScene[] _battleScenes;

    [SerializeField] private UIBattleWindow _battleWindow;
    [SerializeField] private CombatTimer _combatTimer;
    [SerializeField] private CustomGameTimeScaleHandler _customGameTimeScaleHandler;
    [SerializeField] private float _moveToCombatZoneSpeedMultiply;
    private int _currentLevelId => _temploaryInfo.LevelInfo.Id;

    private CameraMovement _camera;
    private CharacterSpeedHolder _characterSpeedHolder;
    private float _charMoveToLvlSpeed;

    [Inject] private CharactersRegistry _charactersRegistry;
    [Inject] private TemploaryInfo _temploaryInfo;

    private bool _isFirstLaunch = true;

    private void Awake()
    {
        _camera = Camera.main.GetComponent<CameraMovement>();

        _characterSpeedHolder = new CharacterSpeedHolder();
        _charMoveToLvlSpeed = _camera.MovementSpeed * _moveToCombatZoneSpeedMultiply;

        OnStartBattle += _uICombatCharacterHolder.CreateCharactersCards;
        OnStartBattle += _battleWindow.Show;

        OnMoveToCombatZone += MoveCharactersToCombatZone;
        OnMoveToCombatZone += MoveCameraToCombatZone;
        OnMoveToCombatZone += _battleWindow.HideImmediate;
        OnMoveToCombatZone += OpenEntry;
        OnCompleteMoveToCombatZone += TryStartFight;

        SetCameraOnSpawnPosition();
    }

    private void MoveCharactersToCombatZone()
    {
        var index = 0;

        foreach (var character in _charactersRegistry.Characters)
        {
            if (character.CharacterView.IsBot)
            {
                character.CharacterView.SetState(new StartBattleState(character.CharacterView, Vector3.zero, this));
            }
            if (!character.CharacterView.IsBot)
            {
                _characterSpeedHolder.SetNewSpeed(character, _charMoveToLvlSpeed);
                character.CharacterView.RestoreCharacter();
                character.CharacterView.UpdateHPBars();
                character.CharacterView.SetState(new StartBattleState
                    (character.CharacterView, _battleScenes[_currentLevelId].StartBattlePositions[index].position, this));
                index++;
            }
        }
    }

    public void OpenEntry()
    {
        if (_currentLevelId == 0) return;

        _battleScenes[_currentLevelId - 1].OpenGates();
    }

    private void MoveCameraToCombatZone()
    {
        var battleScene = _battleScenes[_currentLevelId];
        var waypoints = new List<Transform>();

        if (_isFirstLaunch)
        {
            var cameraState = new CameraMoveState(battleScene.StartCameraPosition, waypoints, battleScene.CameraTarget, battleScene.SpawnCameraRotation, _customGameTimeScaleHandler);
            _camera.SetCameraState(cameraState);
            _camera.Bounds = battleScene.GetSceneBounds();
            _isFirstLaunch = false;
        }
        else
        {
            foreach (var waypoint in battleScene.CameraWaypoints)
            {
                waypoints.Add(waypoint);
            }

            var cameraState = new CameraMoveState(battleScene.StartCameraPosition, waypoints, battleScene.CameraTarget, battleScene.SpawnCameraRotation, _customGameTimeScaleHandler);
            _camera.SetCameraState(cameraState);
            _camera.Bounds = battleScene.GetSceneBounds();
        }
    }

    private void SetCameraOnSpawnPosition()
    {
        _camera.transform.position = _battleScenes[_currentLevelId].SpawnCameraPosition;
        _camera.transform.rotation = _battleScenes[_currentLevelId].SpawnCameraRotation;
    }

    private void TryStartFight()
    {
        if (IsAllCharactersReadyToFight())
        {
            foreach (var character in _charactersRegistry.Characters)
            {
                if (!character.CharacterView.IsBot)
                {
                    _characterSpeedHolder.SetOriginSpeed(character);
                }

                character.CharacterView.SetAttackState();
            }
            OnStartBattle?.Invoke();
            _combatTimer.Activate();
        }
    }

    private bool IsAllCharactersReadyToFight()
    {
        foreach (var character in _charactersRegistry.Characters)
        {
            if (!character.CharacterView.IsBot
                && character.CharacterView.CurrentState is StartBattleState startBattleState
                && !startBattleState.IsReadyToBattle)
            {
                return false;
            }
        }

        return true;
    }

    private void OnDestroy()
    {
        OnMoveToCombatZone = null;
        OnCompleteMoveToCombatZone = null;
        OnStartBattle = null;
    }
}
