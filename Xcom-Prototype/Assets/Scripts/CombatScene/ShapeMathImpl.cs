using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapeMathImpl : IShapeMath, IShapeCollection
{
    private readonly HashSet<IShape> _shapes = new HashSet<IShape>();

    bool IShapeMath.RayCast(Ray ray)
    {
        var hit = _shapes.Any(x => x.IsHit(ray, out _));
        return hit;
    }

    bool IShapeMath.RayCast(Ray ray, out IShape shape)
    {
        var hit = _shapes.Select(x =>
        {
            var isHit = x.IsHit(ray, out var hitDistance);
            return isHit ? (x, hitDistance) : (null, float.MaxValue);
        }).OrderBy(x => x.Item2).FirstOrDefault(x => x.Item1 != null);

        shape = hit.Item1;
        var hitSomething = shape != null;

        return hitSomething;
    }

    void IShapeCollection.Register(IShape shape) => _shapes.Add(shape);

    void IShapeCollection.Remove(IShape shape) => _shapes.Remove(shape);
}