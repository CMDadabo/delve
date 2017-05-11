using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FOVManager : MonoBehaviour
{

    public FOVNode[,] fovMap;

    private BoardManager boardManager;
    private int fovId = 0;


    //TODO: Combine FOVNode and LOSNode, they're kinda redundant
    public class FOVNode
    {
        public float brightness;
        public Color hue;
        public int fovId;

        public FOVNode(float b, int f)
        {
            fovId = f;
            brightness = b;
            hue = Color.black;
        }
    }

    class LOSNode : IEquatable<LOSNode>
    {
        public Vector2 position;
        public float distToOrigin;

        public bool Equals(LOSNode other)
        {
            return Vector2.Equals(position, other.position);
        }

        public override int GetHashCode()
        {
            return position.sqrMagnitude.GetHashCode();
        }


        public LOSNode(Vector2 pos, float dist)
        {
            position = pos;
            distToOrigin = dist;
        }
    }

    public void UpdateLOS(Vector2 origin, int radius)
    {

        fovId++;
        List<LOSNode> nodesInLOS = new List<LOSNode>();

        List<Vector2> rayTargets = Geometry.FindSquaresOnCircle(origin, radius, true).Distinct().ToList();
        rayTargets.ForEach(target =>
        {

            List<LOSNode> nodesInRay = new List<LOSNode>();

            int y0 = (int)origin.y;
            int y1 = (int)target.y;
            int x0 = (int)origin.x;
            int x1 = (int)target.x;

            bool swapXY = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
            int tmp;
            if (swapXY)
            {
                // swap x and y
                tmp = x0;
                x0 = y0;
                y0 = tmp; // swap x0 and y0
                tmp = x1;
                x1 = y1;
                y1 = tmp; // swap x1 and y1
            }

            int deltax = (int)Mathf.Floor(Mathf.Abs(x1 - x0));
            int deltay = (int)Mathf.Floor(Mathf.Abs(y1 - y0));
            var error = Mathf.Floor(deltax / 2);
            var y = y0;
            var ystep = y0 < y1 ? 1 : -1;

            //TODO: Ignore origin square when determining if squares are obstructed
            if (x0 > x1)
            {
                if (swapXY)
                    // Y / X
                    for (int x = x0; x >= x1; x--)
                    {
                        if (0 > x || x >= boardManager.terrainArray.GetLength(0) || 0 > y || y >= boardManager.terrainArray.GetLength(1))
                            break;
                        if (!boardManager.terrainArray[x, y].GetComponent<Tile>().passable)
                            break;
                        nodesInRay.Add(new LOSNode(new Vector2(y, x), 1f));
                        error -= deltay;
                        if (error < 0)
                        {
                            y = y + ystep;
                            error = error + deltax;
                        }
                    }
                else
                    // X / Y
                    for (int x = x0; x >= x1; x--)
                    {
                        if (0 > x || x >= boardManager.terrainArray.GetLength(1) || 0 > y || y >= boardManager.terrainArray.GetLength(0))
                            break;
                        if (!boardManager.terrainArray[y, x].GetComponent<Tile>().passable)
                            break;
                        nodesInRay.Add(new LOSNode(new Vector2(x, y), 1f));
                        error -= deltay;
                        if (error < 0)
                        {
                            y = y + ystep;
                            error = error + deltax;
                        }
                    }
            }
            else
            {
                if (swapXY)
                    // Y / X
                    for (int x = x0; x < x1 + 1; x++)
                    {
                        if (0 > x || x >= boardManager.terrainArray.GetLength(0) || 0 > y || y >= boardManager.terrainArray.GetLength(1))
                            break;
                        if (!boardManager.terrainArray[x, y].GetComponent<Tile>().passable)
                            break;
                        nodesInRay.Add(new LOSNode(new Vector2(y, x), 1f));
                        error -= deltay;
                        if (error < 0)
                        {
                            y = y + ystep;
                            error = error + deltax;
                        }
                    }
                else
                    // X / Y
                    for (int x = x0; x < x1 + 1; x++)
                    {
                        if (0 > x || x >= boardManager.terrainArray.GetLength(1) || 0 > y || y >= boardManager.terrainArray.GetLength(0))
                            break;
                        if (!boardManager.terrainArray[y, x].GetComponent<Tile>().passable)
                            break;
                        nodesInRay.Add(new LOSNode(new Vector2(x, y), 1f));
                        error -= deltay;
                        if (error < 0)
                        {
                            y = y + ystep;
                            error = error + deltax;
                        }
                    }
            }

            if (x0 > x1)
                nodesInRay.Reverse();
            nodesInLOS.AddRange(nodesInRay);
        });

        // Update fovMap
        nodesInLOS.ForEach(node =>
        {
            float brightness = ((float)radius - Vector2.Distance(origin, node.position)) / (float)radius;
            fovMap[(int)node.position.y, (int)node.position.x] = new FOVNode(brightness, fovId);
        });

        // TODO: Further optimize by culling on visibility
        // Update tiles
        if (fovMap != null)
        {
            for (int y = 0; y < fovMap.GetLength(0); y++)
            {
                for (int x = 0; x < fovMap.GetLength(1); x++)
                {
                    var fovNode = fovMap[y, x];
                    if (fovNode != null)
                    {
                        if (fovNode.fovId == fovId - 1)
                        {
                            boardManager.boardTiles[y, x].gameObject.GetComponent<Tile>().SetColor(Color.black);
                            for (int neighbory = y - 1; neighbory <= y + 1; neighbory++)
                            {
                                for (int neighborx = x - 1; neighborx <= x + 1; neighborx++)
                                {
                                    // TODO: Make sure we're in the bounds of the map
                                    if (0 <= neighbory && neighbory < boardManager.boardTiles.GetLength(0) && 0 <= neighborx && neighborx < boardManager.boardTiles.GetLength(1))
                                    {
                                        var tile = boardManager.boardTiles[neighbory, neighborx].gameObject.GetComponent<Tile>();
                                        if (!tile.passable)
                                        {
                                            tile.SetColor(Color.black);
                                        }
                                    }
                                }
                            }
                        }
                        if (fovNode.fovId == fovId)
                        {
                            boardManager.boardTiles[y, x].gameObject.GetComponent<Tile>().SetColor(new Color(fovNode.brightness, fovNode.brightness, fovNode.brightness, 1f));
                            for (int neighbory = y - 1; neighbory <= y + 1; neighbory++)
                            {
                                for (int neighborx = x - 1; neighborx <= x + 1; neighborx++)
                                {
                                    // TODO: Make sure we're in the bounds of the map
                                    if (0 <= neighbory && neighbory < boardManager.boardTiles.GetLength(0) && 0 <= neighborx && neighborx < boardManager.boardTiles.GetLength(1))
                                    {
                                        var tile = boardManager.boardTiles[neighbory, neighborx].gameObject.GetComponent<Tile>();
                                        if (!tile.passable)
                                        {
                                            tile.SetColor(new Color(fovNode.brightness, fovNode.brightness, fovNode.brightness, 1f));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    public float GetBrightnessAt(Vector2 pos)
    {
        if (fovMap[(int)pos.y, (int)pos.x] == null)
            return 0f;
        return fovMap[(int)pos.y, (int)pos.x].brightness;
    }

    // Use this for initialization
    void Start()
    {
        boardManager = GameManager.instance.boardScript;
        fovMap = new FOVNode[boardManager.mapHeight, boardManager.mapWidth];
    }
}
