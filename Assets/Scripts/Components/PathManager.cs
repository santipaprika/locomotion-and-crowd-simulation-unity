using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [HideInInspector]
    public Collider plane;

    [HideInInspector]
    public Vector2 goal;
    public bool moveAgent = true;

    private List<GridCell> path = null;
    private Grid_A_Star gridAStar = null;
    private GridCell startNode = null;
    private Vector2 cellSize;

    // For gizmo visualization
    private float assignedColorHue;

    // Start is called before the first frame update
    void Start()
    {
        // for A*
        GridGenerator gridGenerator = GridGenerator._instance;
        if (gridGenerator)
        {
            // This will define a goal and fill the path variable with the optimal path
            moveAgent = (ComputeNewPath() > 0);

            // Advance to first waypoint (check path found inside)
            goal = getNextWaypoint();
        }
        else
        {
            // assign same plane object as in Crowd Generator object
            plane = GameObject.FindObjectOfType<CrowdGenerator>().plane;

            Vector3 minBound = plane.bounds.min;
            Vector3 maxBound = plane.bounds.max;
            goal = new Vector2(Random.Range(minBound.x, maxBound.x), Random.Range(minBound.z, maxBound.z));
        }

        // Assign random Hue
        assignedColorHue = Random.Range(0f, 1f);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float reachedThr = Mathf.Min(cellSize.x, cellSize.y) / 2f;
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), goal) < reachedThr)
        {
            if (gridAStar != null)
            {
                // if there are no more waypoints, generate a new goal
                if (moveAgent && path.Count == 0)
                {
                    // This will define a new goal and fill the path variable with the optimal path
                    moveAgent = (ComputeNewPath() > 0);
                }

                if (path.Count > 0)
                {
                    // advance to next waypoint
                    goal = getNextWaypoint();
                }
            }
            else
            {
                Vector3 minBound = plane.bounds.min;
                Vector3 maxBound = plane.bounds.max;
                goal = new Vector2(Random.Range(minBound.x, maxBound.x), Random.Range(minBound.z, maxBound.z));
            }
        }
    }

    int ComputeNewPath()
    {
        GridGenerator gridGenerator = GridGenerator._instance;
        GridGraph grid = gridGenerator.grid;
        int found = 1;

        // pick a random node from the available ones to be the goal
        GridCell goalNode = grid.availableNodes[Random.Range(0, grid.availableNodes.Count - 1)];
        goal = new Vector2(goalNode.getCenter().x, goalNode.getCenter().z);

        // define heuristic
        GridHeuristic heuristic = new GridHeuristic(goalNode);

        if (startNode == null)
        {
            // retrieve node index
            cellSize = gridGenerator.gridExtents / (Vector2)gridGenerator.cellsPerDim;
            Vector2 nodeFloatCoords = new Vector2(transform.position.x, transform.position.z) / cellSize;
            int nodeIdx = (int)(nodeFloatCoords.x * gridGenerator.cellsPerDim.y) + (int)nodeFloatCoords.y;
            startNode = grid.nodes[nodeIdx];
        }

        gridAStar = new Grid_A_Star(grid.nodes.Count, 50.0f, 100);
        if (path != null) path.Clear();
        path = gridAStar.findPath(grid, startNode, goalNode, heuristic, ref found);

        // assign start node for recomputing next path
        if (found > 0 && path != null && path.Count > 0)
            startNode = path[path.Count - 1];

        return found;
    }

    Vector2 getNextWaypoint()
    {
        // Don't update if path not found
        if (!moveAgent) return goal;

        Debug.Log(path.Count);
        Vector2 waypoint = new Vector2(path[0].getCenter().x, path[0].getCenter().z);
        path[0].density--;
        path.RemoveAt(0);

        return waypoint;
    }

    void OnDrawGizmosSelected()
    {
        if (!GridGenerator._instance.debugAllAgents)
            DrawGizmos();
    }

    void OnDrawGizmos()
    {
        if (GridGenerator._instance.debugAllAgents)
            DrawGizmos();
    }

    void DrawGizmos()
    {
        // Bright spheres --> Path
        // Dark spheres --> Open nodes when the end node was reached
        // Red prisms --> Character idle because path was not found

        // If you want to debug with Gizmos use a low number of agents! Otherwise it will be a bit messy.

        if (GridGenerator._instance.debugOpenNodes)
        {
            List<Vector3> openNodesCenters = gridAStar.getOpenNodeCenters();
            Gizmos.color = Color.HSVToRGB(assignedColorHue, 0.7f, 0.5f);
            for (int i = 0; i < openNodesCenters.Count; i++)
            {
                Gizmos.DrawSphere(openNodesCenters[i], Mathf.Min(cellSize.x, cellSize.y) / 4f);
            }
        }

        if (GridGenerator._instance.debugPath)
        {
            Gizmos.color = Color.HSVToRGB(assignedColorHue, 0.7f, 1f);
            for (int i = 0; i < path.Count; i++)
            {
                if (i == path.Count - 1)
                {
                    Gizmos.color = Color.HSVToRGB(assignedColorHue, 1f, 1f);

                }
                Gizmos.DrawCube(path[i].getCenter(), new Vector3(cellSize.x, 0.2f, cellSize.y));
            }
        }

        if (!moveAgent)
        {
            Gizmos.DrawCube(transform.position, new Vector3(cellSize.x / 2f, 3f, cellSize.y / 2f));
        }
    }
}
