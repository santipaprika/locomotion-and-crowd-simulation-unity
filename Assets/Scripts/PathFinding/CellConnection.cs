using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PathFinding;
public class CellConnection : Connection<GridCell>
{
    public CellConnection(GridCell from, GridCell to) : base(from, to) {}
}
