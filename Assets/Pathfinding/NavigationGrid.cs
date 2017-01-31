using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Navigation {
	public class NavigationGrid {
		NavNode[,] grid;
		public int X;
		public int Z;

		public NavigationGrid(int x, int z){
			X = x;
			Z = z;
			grid = new NavNode[X,Z];
			for (x = 0; x < X; x++) {
				for (z = 0; z < Z; z++) {
					grid [x, z] = new NavNode ();
				}
			}
		}

		public void updateNavNodeCost(NavNode node, float Cost){
			node.setCost (Cost);
		}
		public void updateNavNodeDepth(NavNode node, int depth){
			node.setDepth (depth);
		}

		public NavNode getNodeAt(int x, int y){
			return grid [x,y];
		}

		public void createDownNodeAt(int x, int y, int depth, float cost){
			if (grid [x,y] != null) {
				if (grid [x,y].getDownNode() != null) {
					NavNode temp = grid [x,y].getDownNode();
					while (temp.getDownNode() != null) {
						temp = temp.getDownNode();
					}
					NavNode newNode = new NavNode (cost, depth);
					temp.setDownNode (newNode);
				} else {
					NavNode newNode = new NavNode (cost, depth);
					grid [x, y].getDownNode ().setDownNode (newNode);
				}
			}
		}
	}

	public class NavNode {
		private NavNode down;
		private int depth;

		private float cost;
		private float angCost;

		public NavNode(){
			down = null;
			depth = 0;
			cost = 1f;
			angCost = Mathf.Sqrt ((cost * cost) + (cost * cost));
		}

		public NavNode(float NavCost, int Depth){
			down = null;
			cost = NavCost;
			angCost = Mathf.Sqrt ((cost * cost) + (cost * cost));
			depth = Depth;
		}

		public NavNode getDownNode(){
			return down;
		}

		public int getDepth(){
			return depth;
		}
		public void setDownNode(NavNode node){
			down = node;
		}

		public void setDepth(int Depth){
			depth = Depth;
		}
		public float getCost(){
			return cost;
		}
		public void setCost (float newCost){
			cost = newCost;
			angCost = Mathf.Sqrt ((newCost * newCost) + (newCost * newCost));
		}
		public float getAngleCost(){
			return angCost;
		}
	}
}