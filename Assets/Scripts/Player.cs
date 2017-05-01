using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Unit {

	private List<Vector2> currentPath;
	private Vector2 moveTarget;
	private FOVManager fovManager;

	void OnDrawGizmos () {


		// Color path when it exists
		if ( currentPath != null ) {
			Gizmos.color = new Color ( 0.85f, 0.75f, 0f, 0.25f );
			currentPath.ForEach ( vector => Gizmos.DrawCube( vector, new Vector3 ( 1, 1, 1 ) ) );
		}
	}

	protected override void Awake ()
	{
		fovManager = GameManager.instance.fovScript;
		base.Awake();
	}

	// Update is called once per frame
	void Update ()
	{

		fovManager.UpdateLOS( transform.position, 5 );

		// Follow player with camera
		Camera.main.transform.position = new Vector3( transform.position.x, transform.position.y, -1 );


		if (Input.GetMouseButtonDown (0)) {
			Vector2 currentCoords = new Vector2( Mathf.Round( transform.position.x ), Mathf.Round( transform.position.y ) );
			currentPath = pathfinder.FindPath( currentCoords, boardManager.SnapMouseToGrid() );
			moveTarget = currentPath[ currentPath.Count - 1 ];
			currentPath.RemoveAt( currentPath.Count - 1 );
		}

		if (currentPath != null) {
			if ( (Vector2)transform.position != moveTarget ) {
				transform.position = Vector2.MoveTowards( transform.position, moveTarget, 10f * Time.deltaTime );
			} else if ( currentPath.Count > 0 ) {
				moveTarget = currentPath[ currentPath.Count - 1 ];
				currentPath.RemoveAt( currentPath.Count - 1 );
			}
		}
	}
}
