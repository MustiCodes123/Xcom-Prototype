using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class BuildingsUIActivator : MonoBehaviour
{
    [Inject] private UIWindowManager _windowsManager;

    public WindowsEnum WindowsEnum;

    private CameraDrag _cameraDrag;
    private Camera _mainCamera;

    public void Setup(CameraDrag cameraDrag)
    {
        _cameraDrag = cameraDrag;
        _mainCamera = Camera.main;
    }

    private void OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!_windowsManager.IsAnyWindowActive() && _cameraDrag.dragOrigin == Input.mousePosition)
        {
            _windowsManager.ShowWindow(WindowsEnum);
        }
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchBegin = new Vector2();

            if(touch.phase == TouchPhase.Began)
            {
                touchBegin = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                RaycastHit hit;
                Ray ray = _mainCamera.ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
                {
                    if (!_windowsManager.IsAnyWindowActive() && touch.position == touchBegin)
                    {
                        _windowsManager.ShowWindow(WindowsEnum);
                    }
                }
            }
        }
    }
}