using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

	private List<Vector2> currentPath;
	private GameObject[,] terrainArray;

	public class Node {

		public Vector2 location;
		public float distanceToStart;
		public float approximateDistanceToEnd;
		public float score;
		public Node parent { get; set; }

		public Node (Vector2 loc, float toStart, float toEnd) {
			location = loc;
			distanceToStart = toStart;
			approximateDistanceToEnd = toEnd;
			score = distanceToStart + approximateDistanceToEnd;
		}
	}

	float GuessDistanceToEnd(Vector2 start, Vector2 end) {
		return Vector2.Distance(start, end);
	}

	List<Vector2> GetAdjacentCoords(Vector2 origin) {
		List<Vector2> adjacentCoords = new List<Vector2>();
		adjacentCoords.Add(origin + new Vector2( 0, 1 ) );
		adjacentCoords.Add(origin + new Vector2( 1, 1 ) );
		adjacentCoords.Add(origin + new Vector2( 1, 0 ) );
		adjacentCoords.Add(origin + new Vector2( 1, -1 ) );
		adjacentCoords.Add(origin + new Vector2( 0, -1 ) );
		adjacentCoords.Add(origin + new Vector2( -1, -1 ) );
		adjacentCoords.Add(origin + new Vector2( -1, 0 ) );
		adjacentCoords.Add(origin + new Vector2( -1, 1 ) );
		return adjacentCoords;
	}

	public List<Vector2> FindPath (Vector2 start, Vector2 end)
	{

		List<Node> openNodes = new List<Node> ();
		List<Node> closedNodes = new List<Node> ();
		Node currentNode;

		if( !terrainArray[ (int)end.y, (int)end.x ].GetComponent<Tile>().passable ) {
			return new List<Vector2>{ start };
		}

		openNodes.Add (new Node (start, 0, GuessDistanceToEnd (start, end)));

		while (openNodes.Count > 0 && !closedNodes.Exists (closedNode => closedNode.location == end)) {

			// Set currentNode to closest to endpoint, move to closed nodes
			openNodes.Sort ((x, y) => x.score.CompareTo (y.score));
			currentNode = openNodes [0];
			openNodes.RemoveAt (0);
			closedNodes.Add (currentNode);

			// Find adjacent nodes
			List<Vector2> adjacentCoords = GetAdjacentCoords (currentNode.location);
			adjacentCoords.ForEach (coord => {
				if (
					!closedNodes.Exists (closedNode => {
						return closedNode.location == coord;
					}) &&
					terrainArray [(int)coord.y, (int)coord.x].GetComponent<Tile> ().passable) {
					if (!openNodes.Exists (openNode => openNode.location == coord)) {
						Node newOpenNode = new Node (
							                   coord,
							                   currentNode.distanceToStart + Vector2.Distance (coord, currentNode.location),
							                   GuessDistanceToEnd (coord, end)
						                   );
						newOpenNode.parent = currentNode;
						openNodes.Add (newOpenNode);
					}
				}
			});
		}

		// Build list of shortest path Vector2 by stepping backwards
		List<Vector2> path = new List<Vector2> ();
		Node endNode = closedNodes.Find (node => node.location == end);

		Node currentPathNode = endNode;
		path.Add(currentPathNode.location);

		while (currentPathNode.parent != null) {
			currentPathNode = currentPathNode.parent;
			path.Add(currentPathNode.location);
		}
		return path;
	}

	void OnDrawGizmos ()
	{
		if (currentPath != null) {
			Gizmos.color = new Color (0f, 1f, 0f, 0.5f);
			currentPath.ForEach (vector => Gizmos.DrawCube (vector, new Vector3 (1, 1, 1)));
		}
	}

	void Start ()
	{
		terrainArray = GameObject.Find ("GameManager").GetComponent<GameManager> ().boardScript.terrainArray;
	}
}
