using UnityEngine;

public class CristalData : MonoBehaviour
{
    public SummonCristalsEnum CristalEnum;
    public string Name; 
    [TextArea(10, 10)]
    public string Description;
    public string Count;
    public Sprite Sprite;
    public RareEnum[] SummonedEnums; 
    public float[] SummonChances = { 0.7f, 0.2f, 0.05f, 0.04f, 0.01f }; //Common, Rare, Epic, Legendary and Mythical
    public CrystalAnimation CrystalAnimation;
}

public enum SummonCristalsEnum
{
    Common,
    Rare,
    Epic,
    Legendary,
    Mythical,
    None
}