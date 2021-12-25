using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathFinding
{

    public class A_Star<TNode, TConnection, TNodeConnection, TGraph, THeuristic> : PathFinder<TNode, TConnection, TNodeConnection, TGraph, THeuristic>
    where TNode : Node
    where TConnection : Connection<TNode>
    where TNodeConnection : NodeConnections<TNode, TConnection>
    where TGraph : FiniteGraph<TNode, TConnection, TNodeConnection>
    where THeuristic : Heuristic<TNode>
    {
        // Class that implements the A* pathfinding algorithm
        // You have to implement the findpath function.
        // You can add whatever you need.

        protected Dictionary<TNode, NodeRecord> visitedNodes; // list of visited nodes 

        protected NodeRecord currentBest; // current best node found

        public enum NodeRecordCategory { OPEN, CLOSED, UNVISITED };

        private SortedList<NodeRecord, NodeRecord> openNodes;

        public class NodeRecord
        {
            // You can use (or not) this structure to keep track of the information that we need for each node

            public NodeRecord() { }

            public int id;
            public TNode node;
            public NodeRecord connection;   // connection traversed to reach this node 
            public float costSoFar; // cost accumulated to reach this node
            public float estimatedTotalCost; // estimated total cost to reach the goal from this node
            public NodeRecordCategory category; // category of the node: open, closed or unvisited
            public int depth; // depth in the search graph
        };

        public class NodeRecordComparer : Comparer<NodeRecord>
        {
            public override int Compare(NodeRecord x, NodeRecord y)
            {
                if (ReferenceEquals(x, y)) return 0;

                float costX = 0f, costY = 0f;
                if (x != null) costX = x.costSoFar + x.estimatedTotalCost;
                if (y != null) costY = y.costSoFar + y.estimatedTotalCost;
                if (costX.CompareTo(costY) == 0)
                {
                    return 1;
                }

                return costX.CompareTo(costY);
            }
        }

        public A_Star(int maxNodes, float maxTime, int maxDepth) : base()
        {
            visitedNodes = new Dictionary<TNode, NodeRecord>();
        }

        public virtual List<TNode> getVisitedNodes()
        {
            return visitedNodes.Keys.ToList();
        }

        static int id = 0;
        public override List<TNode> findPath(TGraph graph, TNode start, TNode end, THeuristic heuristic, ref int found)
        {
            List<TNode> path = new List<TNode>();

            // initialize 'OPEN' sorted list and 'CLOSED' set.
            openNodes = new SortedList<NodeRecord, NodeRecord>(new NodeRecordComparer());
            List<NodeRecord> closed = new List<NodeRecord>();

            // Add 'START' to 'OPEN'
            NodeRecord startRecord = new NodeRecord();
            startRecord.node = start;
            startRecord.connection = null;
            startRecord.costSoFar = 0f;
            startRecord.estimatedTotalCost = heuristic.estimateCost(start);
            startRecord.category = NodeRecordCategory.OPEN;
            startRecord.depth = 0;

            openNodes.Add(startRecord, startRecord);
            visitedNodes[start] = startRecord;

            // Begin iterative exploration
            while (openNodes.ElementAt(0).Value.node != end)
            {
                // Pop node with lower cost and add to CLOSED
                NodeRecord current = openNodes.ElementAt(0).Value;
                openNodes.RemoveAt(0);
                closed.Add(current);

                // Explore current node's neighborhood
                for (int i = 0; i < graph.connections[current.node.id].Count(); i++)
                {
                    TConnection nodeConnection = graph.connections[current.node.id].connections[i];

                    float cost = current.costSoFar + nodeConnection.getCost();

                    bool neighborHasBeenVisited = visitedNodes.ContainsKey(nodeConnection.toNode);
                    if (neighborHasBeenVisited &&
                        visitedNodes[nodeConnection.toNode].category == NodeRecordCategory.OPEN &&
                        cost < visitedNodes[nodeConnection.toNode].costSoFar)
                    {
                        openNodes.Remove(visitedNodes[nodeConnection.toNode]);

                        // change parent, cost and depth
                        visitedNodes[nodeConnection.toNode].connection = current;
                        visitedNodes[nodeConnection.toNode].costSoFar = cost;
                        visitedNodes[nodeConnection.toNode].depth = current.depth + 1;

                        // add modified node record into sorted list
                        openNodes.Add(visitedNodes[nodeConnection.toNode], visitedNodes[nodeConnection.toNode]);
                    }

                    // avoid closed check, since it does not apply to grid

                    // If node wasn't visited yet, create a new node record associated to it
                    if (!neighborHasBeenVisited)
                    {
                        NodeRecord neighborRecord = new NodeRecord();
                        neighborRecord.id = id++;
                        neighborRecord.node = nodeConnection.toNode;
                        neighborRecord.connection = current;
                        neighborRecord.costSoFar = cost;  // g(neighbor)
                        neighborRecord.estimatedTotalCost = heuristic.estimateCost(neighborRecord.node);  // h(neighbor)
                        neighborRecord.category = NodeRecordCategory.OPEN;
                        neighborRecord.depth = current.depth + 1;
                        // Debug.Log("Cost so far: " + neighborRecord.costSoFar + "\nEstimated: " + neighborRecord.estimatedTotalCost);

                        openNodes.Add(neighborRecord, neighborRecord);
                        visitedNodes[nodeConnection.toNode] = neighborRecord;
                    }
                }
            }


            // Store path
            NodeRecord pathNode = openNodes.ElementAt(0).Value;
            while (pathNode.connection != null)
            {
                path.Insert(0, pathNode.node);
                pathNode = pathNode.connection;
            }

            return path;
        }

        public List<Vector3> getOpenNodeCenters()
        {
            List<Vector3> centers = new List<Vector3>();

            for (int i = 0; i < openNodes.Count; i++)
            {
                TNode node = openNodes.ElementAt(i).Value.node;
                centers.Add(node.getCenter());
            }

            return centers;
        }
    };
}