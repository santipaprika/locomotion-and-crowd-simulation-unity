using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PathFinding;
public class GridGraph : FiniteGraph<GridCell, CellConnection, GridNodeConnections>
{
    public GridGraph(Vector2Int cellsPerDim, Vector2 gridDims, float obstacleRate)
    {
        Vector2 cellSize = gridDims / (Vector2)cellsPerDim;
        for (int i = 0; i < cellsPerDim.x; i++)
        {
            for (int j = 0; j < cellsPerDim.y; j++)
            {
                nodes.Add(new GridCell(i, j, cellsPerDim.y, cellSize, obstacleRate));
                connections.Add(new GridNodeConnections(nodes[nodes.Count-1]));
            }
        }

        for (int i = 0; i < cellsPerDim.x - 1; i++)
        {
            for (int j = 0; j < cellsPerDim.y - 1; j++)
            {
                // Each node takes care of connecting the following column and row.
                ConnectNeighbors(i * cellsPerDim.y + j, cellsPerDim.y);
            }
        }
    }

    private void ConnectNeighbors(int idx, int cellsPerRow)
    {
        GridCell currentCell = nodes[idx];
        if (currentCell.isBlocked()) return;

        // right node
        GridCell neighborCell = nodes[idx + 1];
        if (!neighborCell.isBlocked())
        {
            connections[idx].connections.Add(new CellConnection(currentCell, neighborCell));
            connections[idx + 1].connections.Add(new CellConnection(neighborCell, currentCell));
        }

        // bottom node
        neighborCell = nodes[idx + cellsPerRow];
        if (!neighborCell.isBlocked())
        {
            connections[idx].connections.Add(new CellConnection(currentCell, neighborCell));
            connections[idx + cellsPerRow].connections.Add(new CellConnection(neighborCell, currentCell));
        }

        // bottom-right node
        neighborCell = nodes[idx + cellsPerRow + 1];
        if (!neighborCell.isBlocked())
        {
            connections[idx].connections.Add(new CellConnection(currentCell, neighborCell));
            connections[idx + cellsPerRow].connections.Add(new CellConnection(neighborCell, currentCell));
        }
    }
}
