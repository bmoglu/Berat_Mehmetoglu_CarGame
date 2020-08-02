using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level/Create New Level")]
public class Level : ScriptableObject
{
    public int levelNumber;

    public List<PointData> startPoints = new List<PointData>();
    public List<PointData> targetPoints = new List<PointData>();
    public List<PointData> obstacles = new List<PointData>();
}