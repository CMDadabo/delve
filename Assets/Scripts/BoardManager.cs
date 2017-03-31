using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public GameObject wallTile;
	public GameObject floorTile;
	public GameObject[,] terrainArray;
	public int columns = 10;
	public int rows = 10;

	private int[,] testMap = new int[8,8] {
		{1,1,1,1,1,1,1,1},
		{1,0,0,0,0,0,0,1},
		{1,0,0,0,1,1,0,1},
		{1,0,0,0,1,0,0,1},
		{1,0,0,0,1,0,0,1},
		{1,0,1,1,1,0,0,1},
		{1,0,0,0,0,0,0,1},
		{1,1,1,1,1,1,1,1}
	};
	private Transform boardholder;

	private int[,] MakeRoom (int height, int width)
	{
		int[,] room = new int[height, width];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if (y == 0 || y == height - 1 || x == 0 || x == width - 1) {
					room [y, x] = 1;
				} else {
					room [y, x] = 0;
				}
			}
		}

		return room;
	}

	void BoardSetup ()
	{

		int[,] mapToInstantiate = MakeRoom( rows, columns );

		terrainArray = new GameObject[rows, columns];
		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				terrainArray[y,x] = mapToInstantiate [y, x] == 1 ? wallTile : floorTile;
			}
		}

		boardholder = new GameObject ("Board").transform;

		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				GameObject toInstantiate = terrainArray[y,x];
				Instantiate(toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity, boardholder);
			}
		}
	}

	public Vector3 SnapMouseToGrid ()
	{
		Vector3 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return new Vector3(
			Mathf.Round(mouseCoords.x),
			Mathf.Round(mouseCoords.y),
			0
		);
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = new Color (1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube (SnapMouseToGrid (), new Vector3 (1, 1, 1));
	}

	void Start () {
		BoardSetup();
	}
}
