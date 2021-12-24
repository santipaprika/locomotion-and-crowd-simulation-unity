using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PathFinding;
public class Grid_A_Star : A_Star<GridCell, CellConnection, GridNodeConnections, GridGraph, GridHeuristic>
{
    public Grid_A_Star(int maxNodes, float maxTime, int maxDepth) : base(maxNodes, maxTime, maxDepth) {}
}
