using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public GameObject wallTile;
	public GameObject floorTile;
	public GameObject[,] terrainArray;
	public GameObject[,] boardTiles;
	public Vector2[,] litTiles;
	public int mapWidth;
	public int mapHeight;
	public int smoothingRounds;
	public float smoothingThreshold = 0.5f;
	public int mapFullness = 50;
	public int seed = 0;

	private Transform board;
	private FloodFiller floodFiller;
	private int[,] grid;

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

	public Vector2 GetRandomGridCoord ()
	{
		int height = grid.GetLength(0);
		int width = grid.GetLength(1);
		int randomX = Random.Range (0, width);
		int randomY = Random.Range (0, height);

		return new Vector2 (randomX, randomY);
	}


	public Vector2 GetRandomOpenGridCoord ()
	{
		Vector2 randomFillStart = GetRandomGridCoord ();
		while (grid [(int)randomFillStart.y, (int)randomFillStart.x] == 1) {
			randomFillStart = GetRandomGridCoord ();
		}
		return randomFillStart;
	}

	public int[,] GenerateNoiseMap (int height, int width, int percentOpen)
	{
		int[,] noiseMap = new int[height, width];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if (y == 0 || y == height - 1 || x == 0 || x == width - 1) {
					noiseMap [y, x] = 1;
				} else {
					noiseMap [y, x] = percentOpen > Random.Range(0, 100) ? 0 : 1;
				}
			}
		}
		return noiseMap;
	}

	public float NeighborPercentWalls (int checkX, int checkY, int[,] noiseMap)
	{
		int height = noiseMap.GetLength (0);
		int width = noiseMap.GetLength (1);
		List<int> neighborValues = new List<int> ();

		for (int y = checkY - 1; y <= checkY + 1; y++) {
			for (int x = checkX - 1; x <= checkX + 1; x++) {
				// Make sure that the values to check are in the bounds of the array
				if (0 <= x && x < width && 0 <= y && y < height) {
					neighborValues.Add(noiseMap[y, x]);
				}
			}
		}
		return (float)neighborValues.Sum() / neighborValues.Count;
	}

	public int[,] SmoothNoiseMap (int[,] noiseMap, float smoothingThreshold = 0.5f)
	{
		int height = noiseMap.GetLength (0);
		int width = noiseMap.GetLength (1);
		int[,] smoothedMap = new int[height, width];

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				float neighborPercentWalls = NeighborPercentWalls (x, y, noiseMap);
				if (neighborPercentWalls > smoothingThreshold) {
					smoothedMap [y, x] = 1;
				} else if (neighborPercentWalls < smoothingThreshold) {
					smoothedMap [y, x] = 0;
				} else {
					smoothedMap [y, x] = noiseMap [y, x];
				}
			}
		}
		return smoothedMap;
	}

	public virtual void SetupScene ()
	{
		Random.InitState(seed);

		BoardSetup ();

		floodFiller = new FloodFiller( grid );
	}

	int[,] GenerateMapCandidate ()
	{
		int[,] mapCandidate = GenerateNoiseMap (mapHeight, mapWidth, mapFullness);
		for (int i = 1; i <= smoothingRounds; i++) {
			mapCandidate = SmoothNoiseMap (mapCandidate, smoothingThreshold);
		}
		return mapCandidate;
	}

	bool TestMapCandidate ( int[,] grid, float requiredFillPercent )
	{
		Vector2 randomOpenGridCoord = GetRandomOpenGridCoord();
		FloodFiller floodFillTester = new FloodFiller(grid);
		bool [,] filled = floodFillTester.Fill( (int)randomOpenGridCoord.x, (int)randomOpenGridCoord.y );

		int total = 0;
		int totalFilled = 0;
		for( int y = 0; y < filled.GetLength(0); y++) {
			for( int x = 0; x < filled.GetLength(1); x++ ) {
				total++;
				if( filled[y,x]) {
					totalFilled++;
				}
			}
		}

		float filledPercent = (float)totalFilled / (float)total;
		return filledPercent >= requiredFillPercent;
	}

	void BoardSetup ()
	{

		bool passed = false;
		int count = 0;
		bool[,] randomContinuousRegion = new bool[mapHeight,mapWidth];
		while (!passed && count < 30) {
			count++;
			grid = GenerateMapCandidate();
			Vector2 randomOpenGridCoord = GetRandomOpenGridCoord();
			FloodFiller floodFillTester = new FloodFiller(grid);
			randomContinuousRegion = floodFillTester.Fill((int)randomOpenGridCoord.x, (int)randomOpenGridCoord.y);

			float total = 0f;
			float totalFilled = 0f;
			for( int y = 0; y < randomContinuousRegion.GetLength(0); y++) {
				for( int x = 0; x < randomContinuousRegion.GetLength(1); x++ ) {
					total++;
					if( randomContinuousRegion[y,x]) {
						totalFilled++;
					}
				}
			}

			passed = totalFilled/total >= 0.3f;
		}

		terrainArray = new GameObject[mapHeight, mapWidth];
		for (int x = 0; x < mapWidth; x++) {
			for (int y = 0; y < mapHeight; y++) {
				terrainArray[y,x] = randomContinuousRegion [y, x] ? floorTile : wallTile;
			}
		}

		board = new GameObject ("Board").transform;
		boardTiles = new GameObject[mapHeight, mapWidth];

		for (int x = 0; x < mapWidth; x++) {
			for (int y = 0; y < mapHeight; y++) {
				GameObject toInstantiate = terrainArray[y,x];
				boardTiles[y, x] = Instantiate(toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity, board);
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

	void GizmosShowCursorLocation ()
	{
		Gizmos.color = new Color (1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube (SnapMouseToGrid (), new Vector3 (1, 1, 1));
	}

	void OnDrawGizmos ()
	{
		GizmosShowCursorLocation ();
	}
}
