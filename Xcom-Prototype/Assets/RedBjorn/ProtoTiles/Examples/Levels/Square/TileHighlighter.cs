using RedBjorn.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TileHighlighter : MonoBehaviour
{

    // List of GameObjects to map
    [SerializeField] private List<GameObject> gameObjects;

    // Dictionary to store the mapping
    private Dictionary<Vector3, GameObject> positionToGameObjectMap;
    // Start is called before the first frame update
    void Start()
    {
        // Find all GameObjects with the tag "Tile" in the scene and populate the list
        gameObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Tile"));

        // Initialize the dictionary
        positionToGameObjectMap = new Dictionary<Vector3, GameObject>();

        // Populate the dictionary
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
            {
                Vector3 worldPos = obj.transform.position;

                if (!positionToGameObjectMap.ContainsKey(worldPos))
                {
                    positionToGameObjectMap.Add(worldPos, obj);
                }
                else
                {
                    Debug.LogWarning($"Duplicate position {worldPos} for GameObject {obj.name}.");
                }
            }
        }
    }

    /// <summary>
    /// Retrieves the GameObject at the given position, if available.
    /// </summary>
    /// <param name="position">The world position to check.</param>
    /// <returns>The GameObject at the position, or null if none exists.</returns>
    public GameObject GetGameObjectAtPosition(Vector3 position)
    {
        if (positionToGameObjectMap.TryGetValue(position, out GameObject obj))
        {
            return obj;
        }
        else
        {
            Debug.Log($"No GameObject found at position {position}.");
            return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
