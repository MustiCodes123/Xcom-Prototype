using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSetTemplate", menuName = "Data/Items/SetTemplate")]
public class ItemSetTemplate : ScriptableObject
{
    public ItemSetEnum ItemSet;
    public int ItemsToSet;
}
