using System.Collections.Generic;
using UnityEngine;

public class CameraMoveState : ICameraState
{
    private Vector3 _endPosition;
    private List<Transform> _waypoints;
    private int _currentWaypointIndex = 0;
    private Transform _target;
    private CustomGameTimeScaleHandler _timeScaleHandler;
    private Quaternion _quaternion;

    public CameraMoveState(Vector3 endPosition, List<Transform> waypoints, Transform cameraLookTarget, Quaternion quaternion, CustomGameTimeScaleHandler timeScaleHandler)
    {
        _endPosition = endPosition;
        _waypoints = waypoints;
        _target = cameraLookTarget;
        _timeScaleHandler = timeScaleHandler;
        _quaternion = quaternion;
    }

    public void UpdateMovement(CameraMovement cameraMovement)
    {
        if (_waypoints.Count == 0)
        {
            MoveToPosition(cameraMovement, _endPosition, _quaternion);
            return;
        }

        if (_currentWaypointIndex < _waypoints.Count)
        {
            var targetWaypoint = _waypoints[_currentWaypointIndex];
            var targetPosition = targetWaypoint.position;
            var targetRotation = Quaternion.LookRotation(targetWaypoint.position - cameraMovement.transform.position);

            cameraMovement.transform.position = Vector3.MoveTowards(
                cameraMovement.transform.position, targetPosition, cameraMovement.MovementSpeed * Time.deltaTime);

            cameraMovement.transform.rotation = Quaternion.Slerp(
                cameraMovement.transform.rotation, targetRotation, cameraMovement.RotationSpeed * Time.deltaTime);

            if (Vector3.Distance(cameraMovement.transform.position, targetPosition) < 0.1f)
            {
                _currentWaypointIndex++;
            }
        }
        
        // Once all waypoints are traversed, move to the end position
        if (_currentWaypointIndex >= _waypoints.Count)
        {
            MoveToPosition(cameraMovement, _endPosition, _quaternion);
        }
    }

    private void MoveToPosition(CameraMovement cameraMovement, Vector3 position, Quaternion rotation)
    {
        cameraMovement.transform.position = Vector3.MoveTowards(
            cameraMovement.transform.position, position, cameraMovement.MovementSpeed * Time.deltaTime);

        cameraMovement.transform.rotation = Quaternion.Slerp(
            cameraMovement.transform.rotation, rotation, cameraMovement.RotationSpeed * Time.deltaTime);

        if (Vector3.Distance(cameraMovement.transform.position, position) < 0.1f)
        {
            Debug.Log("Reached end position");
#if UNITY_EDITOR
            cameraMovement.SetCameraState(new CameraEditorState(_timeScaleHandler));
#else
            cameraMovement.SetCameraState(new CameraTouchState(_timeScaleHandler));
#endif
        }
    }

}
