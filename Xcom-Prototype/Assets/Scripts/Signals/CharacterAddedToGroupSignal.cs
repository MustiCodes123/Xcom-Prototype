using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterAddedToGroupSignal : ISignal
{
    public BaseCharacterModel CharacterInfo;
    public GroupType GroupType;
}

public enum GroupType
{
    Battle,
    None
}