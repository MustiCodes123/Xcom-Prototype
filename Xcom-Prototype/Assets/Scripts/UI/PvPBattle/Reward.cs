using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Reward
{
    public List<Resource> Resources = new List<Resource>(6);

    public bool Contains(ResourceType type, out Resource resource)
    {
        resource = null;
        foreach (var res in Resources)
        {
            if (res.Type == type)
            {
                resource = res;
                return true;
            }            
        }
        return false;
    }
}

[Serializable]
public class Resource
{
    public ResourceType Type;
    public int Count;

    public ResourceTypeData GetResourceTypeData()
    {
        string typeString = this.Type.ToString();
        Debug.Log($"Type string teext {typeString}");

        if (typeString.EndsWith("Crystal"))
            return ResourceTypeData.Crystal;
        else
            return ResourceTypeData.Currency;
    }
}

public enum ResourceTypeData
{
    Currency,
    Crystal
}
