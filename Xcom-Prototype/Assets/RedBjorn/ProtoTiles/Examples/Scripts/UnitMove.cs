using Microlight.MicroBar;
using RedBjorn.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace RedBjorn.ProtoTiles.Example
{
    public class UnitMove : MonoBehaviour
    {
        public float Speed = 5;
        public float Range = 10f;
        public Transform RotationNode;
        public AreaOutline AreaPrefab;
        public PathDrawer PathPrefab;
        public static float PlayerHealth = 100;
        MapEntity Map;
        AreaOutline Area;
        PathDrawer Path;
        Coroutine MovingCoroutine;
        bool tileShown = false;
        List<TileEntity> oldTiles = new List<TileEntity>();
        public GameObject Enemy;

        public bool playerTurn = false;
        public int turns = 0;

        [SerializeField] MicroBar Simple_MicroBar;
        MicroBar leftMicroBar;


        private void Start()
        {
            leftMicroBar = GameObject.FindGameObjectWithTag("EnemyHealthBar").GetComponent<MicroBar>();
            leftMicroBar.Initialize(100);
            if (leftMicroBar == null) {

                Debug.Log("healthbar not found");
            }

        }
        void Update()
        {
            if(!playerTurn)
            {
                return;
            }
            if (MyInput.GetOnWorldUp(Map.Settings.Plane()))
            {
                HandleWorldClick();
            }
            PathUpdate();
            //TileHide();
        }

        public void Init(MapEntity map)
        {
            Map = map;
            //Area = Spawner.Spawn(AreaPrefab, Vector3.zero, Quaternion.identity);
            AreaShow();
            PathCreate();
        }

        [SerializeField] public TileHighlighter gameObjectMapper; // Reference to GameObjectMapper
        private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();

        /// <summary>
        /// Show tiles and change their color to black, saving the original color.
        /// </summary>
        /// <param name="tiles">List of TileEntity objects.</param>
        public void TileShow(List<TileEntity> tiles)
        {
            //if (tileShown) return;

            if (!oldTiles.SequenceEqual(tiles)) // Compare contents of the lists
            {
                oldTiles = new List<TileEntity>(tiles); // Create a copy of the new tiles list
                TileHide();
            }

            foreach (TileEntity tile in tiles)
            {
                //Debug.Log($"Tile Position: {tile.Position}");

                // Fetch the GameObject at the tile's position
                GameObject targetObject = gameObjectMapper.GetGameObjectAtPosition(tile.Position);
                if (targetObject != null)
                {
                    // Access the first child
                    if (targetObject.transform.childCount > 0)
                    {
                        Transform firstChild = targetObject.transform.GetChild(0);

                        // Get the MeshRenderer of the first child
                        MeshRenderer renderer = firstChild.GetComponent<MeshRenderer>();
                        if (renderer != null)
                        {
                            // Save the original color if not already saved
                            if (!originalColors.ContainsKey(targetObject))
                            {
                                originalColors[targetObject] = renderer.material.color;
                            }

                            // Set the material color to black
                            renderer.material.color = Color.black;
                            //Debug.Log($"Set color to black for: {targetObject.name}");
                        }
                        else
                        {
                            Debug.LogWarning($"No MeshRenderer found on the first child of {targetObject.name}.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"{targetObject.name} has no children.");
                    }
                }
                else
                {
                    Debug.LogWarning($"No GameObject found at position: {tile.Position}");
                }
            }
        }

        /// <summary>
        /// Hide tiles and restore their original color.
        /// </summary>
        private void TileHide()
        {
            //if (!tileShown) return;

            //tileShown = false;

            foreach (var entry in originalColors)
            {
                GameObject targetObject = entry.Key;
                Color originalColor = entry.Value;

                if (targetObject != null && targetObject.transform.childCount > 0)
                {
                    Transform firstChild = targetObject.transform.GetChild(0);
                    MeshRenderer renderer = firstChild.GetComponent<MeshRenderer>();

                    if (renderer != null)
                    {
                        // Restore the original color
                        renderer.material.color = originalColor;
                        //Debug.Log($"Restored color for: {targetObject.name}");
                    }
                }
            }

            // Clear the saved colors after restoring
            originalColors.Clear();
        }

        void HandleWorldClick()
        {
            var clickPos = MyInput.GroundPosition(Map.Settings.Plane());
            var tile = Map.Tile(clickPos);
            if (tile != null && tile.Vacant)
            {
                //AreaHide();
                Path.IsEnabled = false;
                PathHide();
                TileHide();
                var path = Map.PathTiles(transform.position, clickPos, Range);
                Move(path, () =>
                {
                    Path.IsEnabled = true;
                    AreaShow();
                });
                
            }
        }

        public void Move(List<TileEntity> path, Action onCompleted)
        {
            if (path != null)
            {
                if (MovingCoroutine != null)
                {
                    StopCoroutine(MovingCoroutine);
                }
                MovingCoroutine = StartCoroutine(Moving(path, onCompleted));
            }
            else
            {
                onCompleted.SafeInvoke();
            }
        }

        IEnumerator Moving(List<TileEntity> path, Action onCompleted)
        {
            var nextIndex = 0;
            transform.position = Map.Settings.Projection(transform.position);

          
            TileEntity tile = Map.playerTile(Enemy.transform.position);

            int loop = path.Count;

            


            if (path[path.Count - 1].Position == tile.Position)
            {
                loop--;

                var clickPos = MyInput.GroundPosition(Map.Settings.Plane());
                var tile2 = Map.Tile(clickPos);
                if (tile2.Position == tile.Position && path.Count - 1 == 1) {

                    PlayerAttack(10f);
                    loop = 0;
                }
              
            }



            while (nextIndex < loop)
            {
                var targetPoint = Map.WorldPosition(path[nextIndex]);
                var stepDir = (targetPoint - transform.position) * Speed;
                if (Map.RotationType == RotationType.LookAt)
                {
                    RotationNode.rotation = Quaternion.LookRotation(stepDir, Vector3.up);
                }
                else if (Map.RotationType == RotationType.Flip)
                {
                    RotationNode.rotation = Map.Settings.Flip(stepDir);
                }
                var reached = stepDir.sqrMagnitude < 0.01f;
                while (!reached)
                {

                    transform.position += stepDir * Time.deltaTime;
                    reached = Vector3.Dot(stepDir, (targetPoint - transform.position)) < 0f;
                    yield return null;
                }
                transform.position = targetPoint;
                nextIndex++;
            }
            onCompleted.SafeInvoke();
            turns--;
            //Debug.Log("Player turn:")
            if (turns == 0)
            {
                playerTurn = false;
                GamePlayScript.playerEnded = true;

            }
        }

        public void PlayerAttack(float damage)
        {
            float damageAmount = damage;
            AI.EnemyHealth -= damageAmount;
            if (AI.EnemyHealth < 0f) AI.EnemyHealth = 0f;

            if (leftMicroBar != null) leftMicroBar.UpdateBar(AI.EnemyHealth, false, UpdateAnim.Damage);
        }
        //public void PlayerHeal(float healAmount)
        //{
        //    PlayerHealth += healAmount;
        //    if (PlayerHealth > 1) PlayerHealth = 1;

        //    // Update HealthBar
        //    if (leftMicroBar != null) leftMicroBar.UpdateBar(PlayerHealth, false, UpdateAnim.Heal);
        //}
        void AreaShow()
        {
            return;
            AreaHide();
            Area.Show(Map.WalkableBorder(transform.position, Range), Map);
        }

        void AreaHide()
        {
            Area.Hide();
        }

        void PathCreate()
        {
            if (!Path)
            {
                Path = Spawner.Spawn(PathPrefab, Vector3.zero, Quaternion.identity);
                Path.Show(new List<Vector3>() { }, Map);
                Path.InactiveState();
                Path.IsEnabled = true;
            }
        }

        void PathHide()
        {
            if (Path)
            {
                Path.Hide();
            }
        }

        void PathUpdate()
        {
            if (Path && Path.IsEnabled)
            {
                var tile = Map.Tile(MyInput.GroundPosition(Map.Settings.Plane()));
                if (tile != null && tile.Vacant)
                {
                    var path = Map.PathPoints(transform.position, Map.WorldPosition(tile.Position), Range);
                    List<TileEntity> temp = Map.getTiles();
                    
                    
                    
                    Path.Show(path, Map);
                    TileShow(Map.getTiles());
                    Path.ActiveState();

                    //Area.ActiveState();
                }
                else
                {
                    Path.InactiveState();
                    //Area.InactiveState();
                }
            }
        }
    }


   
}
