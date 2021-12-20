using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PathFinding{

	[System.Serializable]
	public abstract class Connection<TNode>
	where TNode : Node
	{
	// Abstract class that represent the connection between 2 nodes
		
		[System.NonSerialized]
		public TNode fromNode; // reference to the origin node
		[System.NonSerialized]
		public TNode toNode; // reference to the destination node
		public int fromNodeId; // id of the origin node
		public int toNodeId; // id of the destination node

		public float cost; // the cost of using that connection in a path 

		public Connection(TNode from, TNode to){
			fromNode = from; 
			toNode = to; 
			fromNodeId = fromNode.id;
			toNodeId = toNode.id;
		}

		public TNode getFromNode(){
			return fromNode;
		}

		public TNode getToNode() {
			return toNode;
		}

		public float getCost() {
			return cost;
		}

		public void setCost(float c){
			cost = c;
		}
	};

}