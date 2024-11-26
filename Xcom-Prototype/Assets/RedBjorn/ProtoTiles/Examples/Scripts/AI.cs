using Microlight.MicroBar;
using RedBjorn.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RedBjorn.ProtoTiles.Example
{
    public class AI : MonoBehaviour
    {
        public float Speed = 5;
        public float Range = 10f;
        public float aiRange = 10f;
        public Transform RotationNode;
        public AreaOutline AreaPrefab;
        public PathDrawer PathPrefab;
        public GameObject player;
        public Animator _animator;
        public UnitMove _unitMoveScript;

        public static float EnemyHealth = 100;

        [SerializeField] MicroBar Simple_MicroBar;
        MicroBar leftMicroBar;

        public bool beenAttacked = false;
        MapEntity Map;
        AreaOutline Area;
        PathDrawer Path;
        Coroutine MovingCoroutine;
        bool tileShown = false;
        List<TileEntity> oldTiles = new List<TileEntity>();

        public bool aiTurn = false;
        public bool moving = false;
        public int turns = 0;
        UnitMove playerScript;


        private void Start()
        {
            playerScript = player.GetComponent<UnitMove>();

            leftMicroBar = GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<MicroBar>();
            leftMicroBar.Initialize(100);
            if (leftMicroBar == null)
            {

                Debug.Log("healthbar not found");
            }
        }

        void Update()
        {
            moveToUser();

            // Get the path from the enemy to the player
            var path = Map.PathTiles(transform.position, player.transform.position, Range);
           
            if (path.Count == 2 && !aiTurn && !playerScript.attacked )
            {
                Debug.Log("set red");
                // Get the tile where the enemy is located
                TileEntity enemyTile = Map.playerTile(transform.position);

                // Fetch the GameObject at the enemy tile's position
                GameObject targetObject = gameObjectMapper.GetGameObjectAtPosition(enemyTile.Position);
                if (targetObject != null && targetObject.transform.childCount > 0)
                {
                    Transform firstChild = targetObject.transform.GetChild(0);
                    MeshRenderer renderer = firstChild.GetComponent<MeshRenderer>();

                    if (renderer != null)
                    {
                        // Save the original color if not already saved
                        if (!originalColors.ContainsKey(targetObject))
                        {
                            originalColors[targetObject] = renderer.material.color;
                        }

                        // Set the material color to red
                        renderer.material.color = Color.red;
                    }
                }
            }
            else
            {
                // Revert the enemy tile to its original color
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
                        }
                    }
                }
               
            }
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
                Debug.Log($"Tile Position: {tile.Position}");

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
                            Debug.Log($"Set color to black for: {targetObject.name}");
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
                        Debug.Log($"Restored color for: {targetObject.name}");
                    }
                }
            }

            // Clear the saved colors after restoring
            originalColors.Clear();
        }

        void moveToUser()
        {
            var clickPos = MyInput.GroundPosition(Map.Settings.Plane());
            var tile = Map.Tile(clickPos);
            
            if (aiTurn && turns > 0 && !moving)
            {
                //AreaHide();
                Path.IsEnabled = false;
                PathHide();
                TileHide();
                
                var path = Map.PathTiles(transform.position,player.transform.position, Range);
                Move(path, () =>
                {
                    Path.IsEnabled = true;
                    //AreaShow();
                });
                turns--;

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
                //moving = true;
            }
            else
            {
                onCompleted.SafeInvoke();
            }
        }

        IEnumerator Moving(List<TileEntity> path, Action onCompleted)
        {
            moving = true;
            int loop = (int)aiRange + 1;
            var nextIndex = 0;
            transform.position = Map.Settings.Projection(transform.position);
            TileEntity tile = Map.playerTile(player.transform.position);
            if (path[path.Count - 1].Position == tile.Position)
            {
                loop = (int)Mathf.Min(path.Count - 1, loop);

                if (turns == 1 && path.Count - 1 == 1) {
                    var targetPoint = Map.WorldPosition(tile);
                    var stepDir = (targetPoint - transform.position) * Speed;
                    if (Map.RotationType == RotationType.LookAt)
                    {
                        RotationNode.rotation = Quaternion.LookRotation(stepDir, Vector3.up);
                    }
                    else if (Map.RotationType == RotationType.Flip)
                    {
                        RotationNode.rotation = Map.Settings.Flip(stepDir);
                    }
                    EnemyAttack(10f);
                    loop = 0;
                    moving = false;
                    if (turns == 0)
                    {
                        aiTurn = false;
                        GamePlayScript.aiEnded = true;
                    }
                    yield return null;
                }
            }
            while (nextIndex < loop)
            {
                _animator.SetFloat("Walk", 1f);
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
                _animator.SetFloat("Walk", 0f);
            }
            onCompleted.SafeInvoke();
            //turns--;
            moving = false;
            if (turns == 0)
            {
                aiTurn = false;
                beenAttacked = false;
                GamePlayScript.aiEnded = true;
            }
        }

        public void EnemyAttack(float damage)
        {
            _animator.SetTrigger("Attack");
            UnitMove.PlayerHealth -= damage;
            if (UnitMove.PlayerHealth < 0f) UnitMove.PlayerHealth = 0f;

            if (leftMicroBar != null) leftMicroBar.UpdateBar(UnitMove.PlayerHealth, false, UpdateAnim.Damage);

            if (UnitMove.PlayerHealth <= 0)
            {
                _unitMoveScript.PlayPlayerDeathAnimation();
            }
        }

        public void PlayEnemyDeathAnimation()
        {

            if (_animator != null)
            {
                _animator.SetTrigger("Die");
            }
        }

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
                
                if (aiTurn)
                {
                    var path = Map.PathPoints(transform.position, player.transform.position, Range);
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
