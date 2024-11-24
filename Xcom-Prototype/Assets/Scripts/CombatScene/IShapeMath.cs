using UnityEngine;

public interface IShapeMath
{
    bool RayCast(Ray ray);
    bool RayCast(Ray ray, out IShape shape);
}