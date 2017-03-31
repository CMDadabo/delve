using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit {

	public int percentChanceToMove = 50;
	public int maxWanderDistance = 5;
	private List<Vector2> currentPath = null;
	private Vector2 moveTarget;

	Vector2 getRandomNearbyCoord (int maxDistance)
	{
		bool passable = false;
		Vector2 target = transform.position;

		while (!passable) {
			target = new Vector2 (
				Mathf.Round(Random.Range (transform.position.x - maxDistance, transform.position.x + maxDistance)),
				Mathf.Round(Random.Range (transform.position.y - maxDistance, transform.position.y + maxDistance))
			);
			passable = boardManager.terrainArray[(int)target.y, (int)target.x].GetComponent<Tile>().passable;
		}

		return target;
	}

	void OnDrawGizmos () {
		if (currentPath != null) {
			Gizmos.color = new Color (0f, 1f, 0f, 0.5f);
			currentPath.ForEach (vector => Gizmos.DrawCube (vector, new Vector3 (1, 1, 1)));
		}
	}

	void Update ()
	{
		if (currentPath == null || moveTarget == (Vector2)transform.position) {
			if (Random.Range (1, 100) > percentChanceToMove) {
				Vector2 currentCoords = new Vector2( Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
				currentPath = pathfinder.FindPath(currentCoords, getRandomNearbyCoord(maxWanderDistance));
				moveTarget = currentPath[ currentPath.Count - 1 ];
				currentPath.RemoveAt(currentPath.Count - 1);
			}
		}

		if (currentPath != null) {
			if ((Vector2)transform.position != moveTarget) {
				transform.position = Vector2.MoveTowards (transform.position, moveTarget, 10f * Time.deltaTime);
			} else if (currentPath.Count > 0) {
				moveTarget = currentPath[ currentPath.Count - 1 ];
				currentPath.RemoveAt(currentPath.Count - 1);
			}
		}

	}
}
