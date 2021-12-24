using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PathFinding;
public class GridHeuristic : Heuristic<GridCell>
{
    // constructor takes a goal node for estimating
    public GridHeuristic(GridCell goal) : base(goal) {}

    // generates an estimated cost to reach the stored goal from the given node
    public override float estimateCost(GridCell fromNode)
    {
        return Vector3.Distance(fromNode.getCenter(), goalNode.getCenter());
    }

    // determines if the goal node has been reached by node
    public override bool goalReached(GridCell node)
    {
        return Vector3.Distance(node.getCenter(), goalNode.getCenter()) < 0.1f;
    }
}