using UnityEngine;

public class CameraTouchState : ICameraState
{
    private CustomGameTimeScaleHandler _timeScaleHandler;
    private bool _isDragging;
    private Vector3 _zoomVelocity;

    public CameraTouchState(CustomGameTimeScaleHandler timeScaleHandler = null)
    {
        _timeScaleHandler = timeScaleHandler;
    }

    public void UpdateMovement(CameraMovement cameraMovement)
    {
        if (_timeScaleHandler != null && _timeScaleHandler.IsPaused) return;

        if (Input.touchCount == 1)
        {
            HandleSingleTouch(cameraMovement);
        }
        else if (Input.touchCount == 2)
        {
            HandlePinchZoom(cameraMovement);
        }
    }

    private void HandleSingleTouch(CameraMovement cameraMovement)
    {
        var touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                StartDragging(cameraMovement, touch);
                break;

            case TouchPhase.Moved:
                ContinueDragging(cameraMovement, touch);
                break;

            case TouchPhase.Ended:
                StopDragging();
                break;
        }
    }

    private void HandlePinchZoom(CameraMovement cameraMovement)
    {
        var touchOne = Input.GetTouch(0);
        var touchTwo = Input.GetTouch(1);

        var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
        var touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;

        var prevTouchDeltaMag = (touchOnePrevPos - touchTwoPrevPos).magnitude;
        var touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;

        var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        var centerPoint = (touchOne.position + touchTwo.position) / 2;
        ZoomCamera(cameraMovement, deltaMagnitudeDiff, centerPoint);
    }

    private void StartDragging(CameraMovement cameraMovement, Touch touch)
    {
        _isDragging = false;
        cameraMovement.TouchOrigin = touch.position;
    }

    private void ContinueDragging(CameraMovement cameraMovement, Touch touch)
    {
        if (!_isDragging)
            _isDragging = Vector2.Distance(touch.position, cameraMovement.TouchOrigin) > cameraMovement.DragThreshold;

        if (_isDragging) MoveCamera(cameraMovement, touch);
    }

    private void StopDragging() => _isDragging = false;

    private Vector3 moveVelocity = Vector3.zero;

    private void MoveCamera(CameraMovement cameraMovement, Touch touch)
    {
        var touchDelta = touch.position - cameraMovement.TouchOrigin;
        var direction = CalculateMovementDirection(cameraMovement, touchDelta);

        var cameraPosition = cameraMovement.transform.position;
        var newPosition = cameraPosition + direction * (cameraMovement.MovementSpeed * Time.unscaledDeltaTime);
        var smoothPosition = Vector3.SmoothDamp(cameraPosition, newPosition, ref moveVelocity, cameraMovement.MoveSmoothTime);

        if (cameraMovement.Bounds.Contains(smoothPosition))
            cameraMovement.transform.position = smoothPosition;

        cameraMovement.TouchOrigin = touch.position;
    }

    private Vector3 CalculateMovementDirection(CameraMovement cameraMovement, Vector2 touchDelta)
    {
        var cameraTransform = cameraMovement.transform;
        var direction = cameraTransform.right * -touchDelta.x + cameraTransform.forward * -touchDelta.y;
        direction.y = 0;

        return direction;
    }

    private void ZoomCamera(CameraMovement cameraMovement, float deltaMagnitudeDiff, Vector2 centerPoint)
    {
        var worldCenterPoint = GetWorldCenterPoint(centerPoint, cameraMovement);
        var newZoom = CalculateNewZoom(cameraMovement, deltaMagnitudeDiff);

        var currentZoom = cameraMovement.transform.position.y;
        var zoomDelta = newZoom - currentZoom;

        var zoomDirection = (cameraMovement.transform.position - worldCenterPoint).normalized;
        var newPosition = cameraMovement.transform.position + zoomDirection * zoomDelta;

        if (cameraMovement.Bounds.Contains(newPosition))
            cameraMovement.transform.position = newPosition;
    }

    private Vector3 GetWorldCenterPoint(Vector2 centerPoint, CameraMovement cameraMovement)
    {
        var ray = cameraMovement.MainCamera.ScreenPointToRay(centerPoint);
        var plane = new Plane(Vector3.up, 0);

        if (plane.Raycast(ray, out var distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    private float CalculateNewZoom(CameraMovement cameraMovement, float deltaMagnitudeDiff)
    {
        var newZoom = cameraMovement.transform.position.y - deltaMagnitudeDiff * cameraMovement.ZoomSpeed;
        return Mathf.Clamp(newZoom, cameraMovement.CurrentMinZoom, cameraMovement.CurrentMaxZoom);
    }
}
