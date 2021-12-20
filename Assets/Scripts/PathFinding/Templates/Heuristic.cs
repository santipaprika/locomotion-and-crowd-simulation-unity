using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PathFinding{

	public abstract class Heuristic <TNode>
	where TNode : Node
	{	
	// Abstract class that represents a Heuristic function to estimate the cost of going from one node to another
	
		protected TNode goalNode; //the goal node that this heuristic is estimating for

		// constructor takes a goal node for estimating
		public	Heuristic(TNode goal){
			goalNode = goal; 
		}

		// generates an estimated cost to reach the stored goal from the given node
		public abstract float estimateCost(TNode fromNode);

		// determines if the goal node has been reached by node
		public abstract bool goalReached (TNode node);

	};

}