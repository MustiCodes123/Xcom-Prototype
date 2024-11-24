using UnityEngine;

public interface IShape
{
    GameObject GameObject { get; }
    bool IsHit(Ray ray, out float hitDistance);
}