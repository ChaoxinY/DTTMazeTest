using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

#region Kruskal
public static class KrusKalDirections
{
	//Basic 2D directions.
	public static readonly Dictionary<string, Vector2Int> directions = new Dictionary<string, Vector2Int>() { { "Up", Vector2Int.up }, { "Down", Vector2Int.down }, { "Left", Vector2Int.left }, { "Right", Vector2Int.right } };
}

public class Edge
{
	public Vector2Int position;
	public Node startNode;
	public Node endNode;
}

//Credit to https://github.com/psholtz/Puzzles/blob/master/mazes/maze-04/java/Kruskal.java for providing this class
public class Tree
{
	private Tree _parent = null;

	public Tree()
	{

	}

	public Tree Root()
	{
		return _parent != null ? _parent.Root() : this;
	}

	public bool Connected(Tree tree)
	{
		return this.Root() == tree.Root();
	}

	public void Connect(Tree tree)
	{
		tree.Root().SetParent(this);
	}


	public void SetParent(Tree parent)
	{
		this._parent = parent;
	}
}

public class Node
{
	public Vector2Int position;
	public Tree rootTree = new Tree();
}

public static partial class MazeCalculatingAlgorithms
{
	public static async Task<List<Vector2Int>> CalculateKruskalMaze(Vector2Int mazeDimensions)
	{
		List<Vector2Int> positions = new List<Vector2Int>();
		List<Node> allNodes = new List<Node>();
		List<Edge> allEdges = new List<Edge>();
		FillCalculationLists(mazeDimensions, allNodes, allEdges, positions);
		while(allEdges.Count != 0)
		{
			Edge currentEdge = allEdges[UnityEngine.Random.Range(0, allEdges.Count)];
			//remove an random edge
			allEdges.Remove(currentEdge);
			//If the cells divided by this edge belong to distinct sets
			if(!currentEdge.startNode.rootTree.Connected(currentEdge.endNode.rootTree))
			{
				//Merge these two node trees into one
				currentEdge.startNode.rootTree.Connect(currentEdge.endNode.rootTree);
				//Render this section
				positions.Add(currentEdge.endNode.position);
				positions.Add(currentEdge.startNode.position);
				positions.Add(currentEdge.position);
			}
		}
		await Task.Delay(1);
		return positions;
	}

	public static void FillCalculationLists(Vector2Int mazeDimensions, List<Node> nodeListToFill, List<Edge> edgeListToFill, List<Vector2Int> positionListToFill)
	{
		int mazeWidth = mazeDimensions.x;
		int mazeHeight = mazeDimensions.y;

		int limit = mazeWidth * mazeHeight;
		for(int i = 0; i < limit; i++)
		{
			int nodeWidth = i % mazeWidth;
			int nodeHeight = i / mazeWidth;
			Vector2Int position = new Vector2Int(nodeWidth*2, nodeHeight*2);
			Node node = new Node();
			node.position = position;
			nodeListToFill.Add(node);
			//Add Edge when
			if(i > 0)
			{
				int leftNodeIndex = nodeListToFill.IndexOf(node)-1;
				if(leftNodeIndex > -1 && nodeWidth !=0)
				{
					Edge edge = new Edge() { startNode = nodeListToFill[leftNodeIndex], endNode = node, position = new Vector2Int(node.position.x + KrusKalDirections.directions["Left"].x, node.position.y) };
					edgeListToFill.Add(edge);
				}
				int upperNodeIndex = nodeListToFill.IndexOf(node)-mazeWidth;
				if(upperNodeIndex > -1)
				{
					Edge edge = new Edge() { startNode = nodeListToFill[upperNodeIndex], endNode = node, position = new Vector2Int(node.position.x, node.position.y + KrusKalDirections.directions["Down"].y) };
					edgeListToFill.Add(edge);
				}
			}
		}
	}
}
#endregion

