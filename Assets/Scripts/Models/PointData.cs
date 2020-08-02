using System;
using UnityEngine;

[Serializable]
public class PointData
{
    public Vector3 position;
    public Quaternion rotation;

    public PointData(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}