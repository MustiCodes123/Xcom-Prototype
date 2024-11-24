using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData", menuName = "Data/ResourceData")]
[Serializable]
public class ResourceData : ScriptableObject
{
    public ResourceType ResourceType;
    public Sprite Sprite;
    public string Name;
}
