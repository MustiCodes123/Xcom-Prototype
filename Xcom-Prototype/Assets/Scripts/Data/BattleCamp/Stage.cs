using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Data/Stage")]
[Serializable]
public class Stage: ScriptableObject 
{
    public int Id;
    public string Name;
    public Sprite Sprite;
    public CampLevel[] Levels;
    public DropCharacter[] DropCharacters;
    public DropCristal DroppedCristal;
    public Stage NextStage;
}
