using UnityEngine;

public class ActiveBuildingsController : MonoBehaviour
{
    [SerializeField] private CameraDrag _cameraDrag;

    [SerializeField] private BuildingsUIActivator[] _activeBuildings;

    private void Start()
    {
        SetupBuildings();
    }

    private void SetupBuildings()
    {
        foreach(var building in _activeBuildings)
        {
            building.Setup(_cameraDrag);
        }
    }
}
