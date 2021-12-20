using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PathFinding;

public class GridNodeConnections : NodeConnections<GridCell, CellConnection>
{
    protected GridCell cell;

    public GridNodeConnections(GridCell cell) {
        this.cell = cell;
    }

    public void Connect(GridCell otherCell) {
        connections.Add(new CellConnection(cell, otherCell));
    }
}
