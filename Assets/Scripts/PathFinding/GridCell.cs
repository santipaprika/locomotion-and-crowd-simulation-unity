using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PathFinding;

public class GridCell : Node
{
    // Your class that represents a grid cell node derives from Node
    // You add any data needed to represent a grid cell node
    protected float xMin;
    protected float xMax;
    protected float zMin;
    protected float zMax;
    protected bool occupied;
    protected bool blocked;
    protected Vector3 center;

    public GridCell(int i, int j, int cellsPerRow, Vector2 dims, float obstacleProbability) : base(i*cellsPerRow + j) {
        blocked = Random.Range(0.0f,1.0f) < obstacleProbability;

        center = new Vector3(i * dims.x, 0, j * dims.y);
        xMin = center.x - dims.x/2.0f;  
        xMax = center.x - dims.x/2.0f;  
        zMin = center.z - dims.y/2.0f;  
        zMax = center.z - dims.y/2.0f;  
    }

    public bool isBlocked() {
        return blocked;
    }

    public Vector3 getCenter() {
        return center;
    }
}


