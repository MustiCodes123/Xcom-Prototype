using System;
using UnityEngine;

public class PointDecale : AOEDecale
{
    [SerializeField] private Transform _attractionPoint;

    public Transform AttractionPoint => _attractionPoint;
}