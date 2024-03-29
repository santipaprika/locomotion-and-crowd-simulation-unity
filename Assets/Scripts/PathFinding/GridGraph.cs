﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PathFinding;
public class GridGraph : FiniteGraph<GridCell, CellConnection, GridNodeConnections>
{
    public List<GridCell> availableNodes;
    public Vector2 cellSize;
    enum BorderPoint { None, X, Y };
    public GridGraph(Vector2Int cellsPerDim, Vector2 gridDims, float obstacleRate)
    {
        availableNodes = new List<GridCell>();

        cellSize = gridDims / (Vector2)cellsPerDim;
        for (int i = 0; i < cellsPerDim.x; i++)
        {
            for (int j = 0; j < cellsPerDim.y; j++)
            {
                GridCell cell = new GridCell(i, j, cellsPerDim.y, cellSize, obstacleRate);
                nodes.Add(cell);
                connections.Add(new GridNodeConnections(cell));
                
                if (!nodes[nodes.Count-1].isBlocked()) {
                    availableNodes.Add(cell);
                }
            }
        }

        for (int i = 0; i < cellsPerDim.x; i++)
        {
            for (int j = 0; j < cellsPerDim.y - 1; j++)
            {
                if (i < cellsPerDim.x - 1)
                {
                    // Each node takes care of connecting the following column and row.
                    ConnectNeighbors(i * cellsPerDim.y + j, cellsPerDim.y);
                }
                else
                {
                    // Connect last row only with horizontal neighbors
                    ConnectNeighbors(i * cellsPerDim.y + j, cellsPerDim.y, BorderPoint.X);
                }
            }

            if (i < cellsPerDim.x - 1)
            {
                // Connect last column only with vertical neighbors only if we are not in the last row
                ConnectNeighbors((i + 1) * cellsPerDim.y - 1, cellsPerDim.y, BorderPoint.Y);
            }
        }

        // Instantiate walls inside parent 'Map' GameObject
        GameObject mapGO = new GameObject("Map");
        for(int i = 0; i < nodes.Count; i++) {
            GridCell cell = nodes[i];
            if (cell.isBlocked()) {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.transform.position = cell.getCenter();
                wall.transform.localScale = new Vector3(cellSize.x, 3, cellSize.y);
                wall.transform.parent = mapGO.transform;
            }
        }
    }

    private void ConnectNeighbors(int idx, int cellsPerRow, BorderPoint borderPoint = BorderPoint.None)
    {
        GridCell currentCell = nodes[idx];
        if (currentCell.isBlocked()) return;

        // see if both sides are not blocked to create diagonal path
        bool sidesAvailable = false;

        GridCell neighborCell;

        // if it is not last column, connect with following column's node
        if (borderPoint != BorderPoint.Y)
        {
            // right node
            neighborCell = nodes[idx + 1];
            if (!neighborCell.isBlocked())
            {
                connections[idx].connections.Add(new CellConnection(currentCell, neighborCell));
                connections[idx + 1].connections.Add(new CellConnection(neighborCell, currentCell));

                sidesAvailable = true;
            }
        }

        // if it is not last row, connect with following row's node
        if (borderPoint != BorderPoint.X)
        {
            // bottom node
            neighborCell = nodes[idx + cellsPerRow];
            if (!neighborCell.isBlocked())
            {
                connections[idx].connections.Add(new CellConnection(currentCell, neighborCell));
                connections[idx + cellsPerRow].connections.Add(new CellConnection(neighborCell, currentCell));
            }
            else
            {
                sidesAvailable = false;
            }
        }

        // if it is neither last column or row, connect with following column's  and row diagonal node
        if (borderPoint == BorderPoint.None)
        {
            // bottom-right node
            neighborCell = nodes[idx + cellsPerRow + 1];
            if (!neighborCell.isBlocked() && sidesAvailable && borderPoint == BorderPoint.None)
            {
                // Diagonal 1
                connections[idx].connections.Add(new CellConnection(currentCell, neighborCell));
                connections[idx + cellsPerRow + 1].connections.Add(new CellConnection(neighborCell, currentCell));

                // Diagonal 2
                connections[idx + 1].connections.Add(new CellConnection(nodes[idx + 1], nodes[idx + cellsPerRow]));
                connections[idx + cellsPerRow].connections.Add(new CellConnection(nodes[idx + cellsPerRow], nodes[idx + 1]));
            }
        }
    }
}
