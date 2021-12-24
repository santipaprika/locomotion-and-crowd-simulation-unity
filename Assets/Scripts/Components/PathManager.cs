using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [HideInInspector]
    public Collider plane;

    [HideInInspector]
    public Vector2 goal;

    // Start is called before the first frame update
    void Start()
    {
        // for A*
        GridGenerator gridGenerator = GridGenerator._instance;
        if (gridGenerator)
        {
            GridGraph grid = gridGenerator.grid;
            int found = 1; // init as not found

            // pick a random node from the available ones to be the goal
            GridCell goalNode = grid.availableNodes[Random.Range(0,grid.availableNodes.Count-1)];
            goal = new Vector2(goalNode.getCenter().x, goalNode.getCenter().z);

            // define heuristic
            GridHeuristic heuristic = new GridHeuristic(goalNode);

            // retrieve node index
            Vector2 cellSize = gridGenerator.gridExtents/(Vector2)gridGenerator.cellsPerDim;
            Vector2 nodeFloatCoords = new Vector2(transform.position.x, transform.position.z) / cellSize;
            int nodeIdx = (int)(nodeFloatCoords.x*gridGenerator.cellsPerDim.y) + (int)nodeFloatCoords.y;
            List<GridCell> path = gridGenerator.gridAStar.findPath(grid, grid.nodes[nodeIdx], goalNode, heuristic, ref found);
        }
        else
        {
           // assign same plane object as in Crowd Generator object
            plane = GameObject.FindObjectOfType<CrowdGenerator>().plane;

            Vector3 minBound = plane.bounds.min;
            Vector3 maxBound = plane.bounds.max;
            goal = new Vector2(Random.Range(minBound.x, maxBound.x), Random.Range(minBound.z, maxBound.z));
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GridGenerator._instance)
        {
        }
        else
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), goal) < 0.1f)
            {
                Vector3 minBound = plane.bounds.min;
                Vector3 maxBound = plane.bounds.max;
                goal = new Vector2(Random.Range(minBound.x, maxBound.x), Random.Range(minBound.z, maxBound.z));
            }
        }
    }
}
