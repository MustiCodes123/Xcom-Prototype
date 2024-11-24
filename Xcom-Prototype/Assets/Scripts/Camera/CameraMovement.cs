using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float MovementSpeed = 3f;
    public float RotationSpeed = 3f;

    [Header("Zoom Settings")]
    public float ZoomSpeed = 0.1f;
    public float MinZoom = 1f; 
    public float MaxZoom = 10f; 

    [Header("Editor Settings")]
    public float DragTimeThreshold = 0.2f;
    public float ZoomLerpSpeed = 10f;
    public float MovementLerpSpeed = 10f;

    [Header("Touch Settings")]
    public float DragThreshold = 0.1f;
    public float ZoomSmoothTime = 0.1f;
    public float MoveSmoothTime = 0.1f;

    public Bounds Bounds { get; set; } = new Bounds();
    public Vector2 TouchOrigin { get; set; } = -Vector2.one;
    public float CurrentMaxZoom { get; private set; }
    public float CurrentMinZoom { get; private set; }
    public Camera MainCamera{  get; private set; }

    private ICameraState _currentState;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        CalculateZoomLimits();
        MainCamera = Camera.main;
    }

    private void OnEnable()
    {
        SkillView.DecaleDragStateChanged += OnSkillPointerHandlerChanged;
    }

    private void OnDisable()
    {
        SkillView.DecaleDragStateChanged -= OnSkillPointerHandlerChanged;
    }

    public void SetCameraState(ICameraState state)
    {
        _currentState = state;
    }

    private void LateUpdate()
    {
        if (_currentState != null)
            _currentState.UpdateMovement(this);
    }

    private void CalculateZoomLimits()
    {
        var position = _transform.localPosition;
        CurrentMinZoom = position.y - MinZoom;
        CurrentMaxZoom = position.y + MaxZoom;
    }


    private void OnSkillPointerHandlerChanged(bool value)
    {
        if (value)
        {
            SetCameraState(new CameraStopState());
            return;
        }

#if UNITY_EDITOR
        SetCameraState(new CameraEditorState());
#else
        SetCameraState(new CameraTouchState());
#endif
    }
}
