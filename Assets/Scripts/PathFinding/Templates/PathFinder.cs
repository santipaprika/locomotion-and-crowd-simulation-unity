using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PathFinding{

	public abstract class PathFinder <TNode, TConnection, TNodeConnections, TGraph, THeuristic> 
	where TNode : Node
	where TConnection : Connection<TNode>
	where TNodeConnections : NodeConnections<TNode,TConnection>
	where TGraph : Graph<TNode,TConnection,TNodeConnections>
	where THeuristic : Heuristic<TNode>
	{
	// Abstract class that represents a path finding algorithm.
	// It must have a function to find a path between two nodes using an heuristic function 
		
		public	PathFinder(){}

		// returns, if found, a path from start to end in graph using heuristic
		// found < 0 --> path not found
		// found == 0 --> path not found or incomplete
		// found > 0 --> path found
		public abstract List<TNode> findpath(TGraph graph, TNode start, TNode end, THeuristic heuristic, ref int found); 
	};

}