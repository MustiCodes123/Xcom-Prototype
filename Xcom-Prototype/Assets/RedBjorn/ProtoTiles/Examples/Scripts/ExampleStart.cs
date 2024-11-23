using UnityEngine;

namespace RedBjorn.ProtoTiles.Example
{
    public class ExampleStart : MonoBehaviour
    {
        public MapSettings Map;
        public KeyCode GridToggle = KeyCode.G;
        public MapView MapView;
        public UnitMove Unit;
        public AI EnemyUnit;
        public GamePlayScript gamePlayScript;
        public MapEntity MapEntity { get; private set; }

        void Start()
        {
            // Limit frame rate to match the monitor's refresh rate (e.g., 60Hz)
            Application.targetFrameRate = 60;
            if (!MapView)
            {
#if UNITY_2023_1_OR_NEWER
                MapView = FindFirstObjectByType<MapView>();
#else
                MapView = FindObjectOfType<MapView>();
#endif
            }
            MapEntity = new MapEntity(Map, MapView);
            if (MapView)
            {
                MapView.Init(MapEntity);
            }
            else
            {
                Log.E("Can't find MapView. Random errors can occur");
            }

            if (!Unit && !EnemyUnit)
            {
#if UNITY_2023_1_OR_NEWER
                Unit = FindFirstObjectByType<UnitMove>();
                EnemyUnit = FindFirstObjectByType<AI>();
#else
                Unit = FindObjectOfType<UnitMove>();
                EnemyUnit = FindObjectOfType<AI>();
#endif
            }
            if (Unit && EnemyUnit)
            {
                Unit.Init(MapEntity);
                EnemyUnit.Init(MapEntity);
            }
            else
            {
                Log.E("Can't find any Unit. Example level start incorrect");
            }
        }

        void Update()
        {
            if (Input.GetKeyUp(GridToggle))
            {
                MapEntity.GridToggle();
            }
        }

    }

}
