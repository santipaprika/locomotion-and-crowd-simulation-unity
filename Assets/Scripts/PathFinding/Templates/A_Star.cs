using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathFinding{

	public class A_Star<TNode,TConnection,TNodeConnection,TGraph,THeuristic> : PathFinder<TNode,TConnection,TNodeConnection,TGraph,THeuristic>
	where TNode : Node
	where TConnection : Connection<TNode>
	where TNodeConnection : NodeConnections<TNode,TConnection>
	where TGraph : Graph<TNode,TConnection,TNodeConnection>
	where THeuristic : Heuristic<TNode>
	{
	// Class that implements the A* pathfinding algorithm
	// You have to implement the findpath function.
	// You can add whatever you need.
				
		protected List<TNode> visitedNodes; // list of visited nodes 
		
		protected NodeRecord currentBest; // current best node found
		
		protected enum NodeRecordCategory{ OPEN, CLOSED, UNVISITED };
				
		protected class NodeRecord{	
		// You can use (or not) this structure to keep track of the information that we need for each node
			
			public NodeRecord(){}
			
			public TNode node; 
			public NodeRecord connection;	// connection traversed to reach this node 
			public float costSoFar; // cost accumulated to reach this node
			public float estimatedTotalCost; // estimated total cost to reach the goal from this node
			public NodeRecordCategory category; // category of the node: open, closed or unvisited
			public int depth; // depth in the search graph
		};

		public	A_Star(int maxNodes, float maxTime, int maxDepth):base(){ 
			
			visitedNodes = new List<TNode> ();
			
		}

		public virtual List<TNode> getVisitedNodes(){
			return visitedNodes;
		}
		
		public override List<TNode> findpath(TGraph graph, TNode start, TNode end, THeuristic heuristic, ref int found)
		{
			List<TNode> path = new List<TNode>();
			
			// TO IMPLEMENT

			return path;
		}

	};

}