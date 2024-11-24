using UnityEngine;
using Zenject;

public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 2;
    public Vector3 dragOrigin;

    public bool cameraDragging = true;

    public float outerLeft = -10f;
    public float outerRight = 10f;
    public float outerTop = 10f;
    public float outerDown = 10f;

    [SerializeField] private float _mouseWheelsensivity = 10;
    [SerializeField] private float _touchZoomlsensivity = 0.01f;
    [SerializeField] private float _zoomMin = 20;
    [SerializeField] private float _zoomMax = 50;

    private const string _mouseWhellName = "Mouse ScrollWheel";

    [Inject] private UIWindowManager _windowsManager;

    private void Start()
    {
        _touchZoomlsensivity *= 10;
    }

    private void Update()
    {
        DragCameraOnInput();

        if (Input.touchCount > 1)
        {
            ZoomCameraByInputTouch();
        }

        if (Input.GetAxis(_mouseWhellName) != 0)
        {
            ZoomCameraByMouseScroll();
        }
    }

    private void DragCameraOnInput()
    {
        if (gameObject.activeInHierarchy == false || Camera.main == null || _windowsManager.IsAnyWindowActive())
        {
            return;
        }

        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        float left = Screen.width * 0.2f;
        float right = Screen.width - (Screen.width * 0.2f);

        if (mousePosition.x < left)
        {
            cameraDragging = true;
        }
        else if (mousePosition.x > right)
        {
            cameraDragging = true;
        }

        if (cameraDragging)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = Input.mousePosition;

                return;
            }

            if (!Input.GetMouseButton(0))
            {
                return;
            }

            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

            Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);

            transform.Translate(move, Space.World);

            if (transform.position.x > outerRight)
            {
                transform.transform.position = new Vector3(outerRight, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < outerLeft)
            {
                transform.transform.position = new Vector3(outerLeft, transform.position.y, transform.position.z);
            }

            if (transform.position.z > outerTop)
            {
                transform.transform.position = new Vector3(transform.position.x, transform.position.y, outerTop);
            }
            else if (transform.position.z < outerDown)
            {
                transform.transform.position = new Vector3(transform.position.x, transform.position.y, outerDown);
            }
        }
    }

    private void ZoomCamera(float increment)
    {
        Camera mainCamera = Camera.main;
        if(mainCamera == null)
        {
            return;
        }
        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView - increment, _zoomMin, _zoomMax);
    }

    private void ZoomCameraByInputTouch()
    {
        Touch touchA = Input.GetTouch(0);
        Touch touchB = Input.GetTouch(1);

        Vector2 touchDirrectionA = touchA.position - touchA.deltaPosition;
        Vector2 touchDirrectionB = touchB.position - touchB.deltaPosition;

        float prevMagnitude = (touchDirrectionA - touchDirrectionB).magnitude;
        float currMagnitude = (touchA.position - touchB.position).magnitude;

        float difference = currMagnitude - prevMagnitude;

        ZoomCamera(difference * _touchZoomlsensivity);
    }

    private void ZoomCameraByMouseScroll()
    {
        float zoom = Input.GetAxis(_mouseWhellName);
        ZoomCamera(zoom * _mouseWheelsensivity);
    }
}