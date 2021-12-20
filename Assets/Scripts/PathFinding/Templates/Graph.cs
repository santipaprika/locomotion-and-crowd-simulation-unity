using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PathFinding{
	
	[System.Serializable]
	public abstract class Graph<TNode, TConnection, TNodeConnections>
	where TNode : Node
	where TConnection : Connection<TNode>
	where TNodeConnections : NodeConnections<TNode,TConnection>
	{	
	// Abstract class that represents a graph (infinite or not), that is at least a function that returns a list of connection for any node
	
		public	Graph(){

		}
	
		public abstract TNodeConnections getConnections (TNode fromNode);	

	};


	[System.Serializable]
	public abstract class FiniteGraph<TNode, TConnection, TNodeConnections> : Graph<TNode, TConnection, TNodeConnections>
	where TNode : Node
	where TConnection : Connection<TNode>
	where TNodeConnections : NodeConnections<TNode,TConnection>
	{	
	// Abstract class that represents a finite graph, where there is a known set of nodes and a known set of connections between those nodes.
	
		[SerializeField] public List<TNode> nodes;
		[SerializeField] public List<TNodeConnections> connections;
		
		public	FiniteGraph():base(){
			nodes = new List<TNode> ();
			connections = new List<TNodeConnections> ();
		}
		
		public TNodeConnections getConnections(int fromNodeIndex) { 
			return connections[fromNodeIndex];
		}
		
		public override TNodeConnections getConnections(TNode fromNode) {
			return getConnections (fromNode.getId ());
		}
		
		public int getNumNodes() { return nodes.Count; }
		public TNode getNode(int i)  { 
			if (i < getNumNodes())
				return nodes[i];
			else
				return null;
		}
	};

}