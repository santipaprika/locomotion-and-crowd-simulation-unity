using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Vector2Int cellsPerDim = new Vector2Int(10, 10);
    public Vector2 gridExtents = new Vector2(10, 10);
    public float obstacleRate = 0.2f;
    private GridGraph grid;

    // Start is called before the first frame update
    void Start()
    {
        // assign same plane object as in Crowd Generator object
        // Collider plane = GameObject.FindObjectOfType<CrowdGenerator>().plane;
        // Vector3 extents = plane.bounds.extents;

        grid = new GridGraph(cellsPerDim, new Vector2(gridExtents.x, gridExtents.y), obstacleRate);

        for(int i = 0; i < grid.nodes.Count; i++) {
            GridCell cell = grid.nodes[i];
            if (cell.isBlocked()) {
                Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), cell.getCenter(), Quaternion.identity);
            }
        }    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
