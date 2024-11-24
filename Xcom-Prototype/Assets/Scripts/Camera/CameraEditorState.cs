using UnityEngine;

public class CameraEditorState : ICameraState
{
    private readonly CustomGameTimeScaleHandler _timeScaleHandler;
    private Vector3 _targetPosition;
    private float _targetZoom;
    private bool _isDragging;
    private float _dragTime;

    public CameraEditorState(CustomGameTimeScaleHandler timeScaleHandler = null)
    {
        _timeScaleHandler = timeScaleHandler;
    }

    public void UpdateMovement(CameraMovement cameraMovement)
    {
        if (_timeScaleHandler != null && _timeScaleHandler.IsPaused) return;

        HandleMouseInput(cameraMovement);
        HandleZoom(cameraMovement);
        SmoothZoomCamera(cameraMovement);
    }

    private void HandleMouseInput(CameraMovement cameraMovement)
    {
        if (Input.GetMouseButtonDown(0)) StartDragging(cameraMovement);
        if (Input.GetMouseButton(0)) ContinueDragging(cameraMovement);

        if (Input.GetMouseButtonUp(0)) StopDragging();
    }

    #region MoveCamera
    private void StartDragging(CameraMovement cameraMovement)
    {
        _dragTime = 0f;
        cameraMovement.TouchOrigin = Input.mousePosition;
    }

    private void ContinueDragging(CameraMovement cameraMovement)
    {
        _dragTime += Time.deltaTime;

        if (!(_dragTime > cameraMovement.DragTimeThreshold)) return;
        _isDragging = true;
        var mouseDelta = (Vector2)Input.mousePosition - cameraMovement.TouchOrigin;
        
        MoveCamera(cameraMovement, mouseDelta);
        
        cameraMovement.TouchOrigin = Input.mousePosition;
    }

    private void StopDragging() => _isDragging = false;
    
    private void MoveCamera(CameraMovement cameraMovement, Vector2 delta)
    {
        var direction = CalculateMovementDirection(cameraMovement, delta);
        _targetPosition = CalculateTargetPosition(cameraMovement, direction);

        if (!cameraMovement.Bounds.Contains(_targetPosition)) return;
        cameraMovement.transform.position = Vector3.Lerp(cameraMovement.transform.position, _targetPosition, Time.unscaledDeltaTime * cameraMovement.MovementLerpSpeed);
    }
    
    private Vector3 CalculateMovementDirection(CameraMovement cameraMovement, Vector2 delta)
    {
        var cameraTransform = cameraMovement.transform;
        
        var direction = cameraTransform.right * -delta.x + cameraTransform.forward * -delta.y;
        direction.y = 0;
        
        return direction;
    }
    
    #endregion

    #region ZoomCamera
    private void HandleZoom(CameraMovement cameraMovement)
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0f) return;
        
        _targetZoom = CalculateTargetZoom(cameraMovement, scroll);
    }

    private float CalculateTargetZoom(CameraMovement cameraMovement, float scroll)
    {
        var targetZoom = cameraMovement.transform.position.y - scroll * cameraMovement.ZoomSpeed * 100f;
        
        return Mathf.Clamp(targetZoom, cameraMovement.CurrentMinZoom, cameraMovement.CurrentMaxZoom);
    }
    
    private Vector3 CalculateTargetPosition(CameraMovement cameraMovement, Vector3 direction)
    {
        return cameraMovement.transform.position + direction * (cameraMovement.MovementSpeed * Time.unscaledDeltaTime);
    }

    private void SmoothZoomCamera(CameraMovement cameraMovement)
    {
        var cameraPosition = cameraMovement.transform.position;
        var newZoom = Mathf.Lerp(cameraPosition.y, _targetZoom, Time.unscaledDeltaTime * cameraMovement.ZoomLerpSpeed);
        newZoom = Mathf.Clamp(newZoom, cameraMovement.CurrentMinZoom, cameraMovement.CurrentMaxZoom);
        
        var newPosition = new Vector3(cameraPosition.x, newZoom, cameraPosition.z);

        if (cameraMovement.Bounds.Contains(newPosition))
            cameraMovement.transform.position = newPosition;
    }
    
    #endregion
 
}
