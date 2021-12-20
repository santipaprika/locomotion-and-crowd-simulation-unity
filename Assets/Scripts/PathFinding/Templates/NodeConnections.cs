using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PathFinding{

	[System.Serializable]
	public class NodeConnections<TNode,TConnection>
	where TNode : Node
	where TConnection : Connection<TNode>
	{
	// Class to handle a list of connections 
		
		public List<TConnection> connections;
		
		public NodeConnections(){
			
			connections = new List<TConnection>();
			
		}
		
		public TConnection ElementAt(int key){
			return connections [key];
		}

		public void SetElementAt(int key, TConnection c){
			connections [key] = c;
		}
		
		public int Count() {
			return connections.Count;
		}
		
		public void Clear(){
			connections.Clear ();
		}
		
		public void Add(TConnection c){
			connections.Add (c);
		}

		public bool Exists(TConnection c){
			bool found = false;
			int i = 0;
			while (i < connections.Count && !found) {
				found = connections[i].fromNodeId == c.fromNodeId && connections[i].toNodeId == c.toNodeId;
				i++;
			}
			return found;
		}
	};

}