using UnityEngine;

public class CombatTestBoardManager : BoardManager
{

    public override void SetupScene()
    {
        Transform board = new GameObject("Board").transform;
        boardTiles = new GameObject[mapHeight, mapWidth];
        terrainArray = new GameObject[mapHeight, mapWidth];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                terrainArray[y, x] = floorTile;
                boardTiles[y, x] = Instantiate(floorTile, new Vector3(x, y, 0f), Quaternion.identity, board);
            }
        }
    }
}
