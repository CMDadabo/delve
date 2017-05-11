using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public int speed; // ft/sec
    public bool takingTurn;

    protected Pathfinder pathfinder;
    protected BoardManager boardManager;
    protected FOVManager fovManager;
    protected int movesRemaining;
    protected List<Vector2> currentPath;
    protected Vector2 moveTarget;

    protected virtual bool Sees(Unit target)
    {
        return HasLineOfSightTo(target) && target.IsLit();
    }

    protected virtual bool HasLineOfSightTo(Unit target)
    {
        List<Vector2> lineOfSight = Geometry.DrawLine(transform.position, target.transform.position);
        for (int i = 0; i < lineOfSight.Count; i++)
        {
            if (!boardManager.terrainArray[(int)lineOfSight[i].y, (int)lineOfSight[i].x].gameObject.GetComponent<Tile>().passable)
                return false;
        }
        return true;
    }

    protected virtual bool IsLit()
    {
        return GetBrightness() > 0f;
    }

    protected virtual float GetBrightness()
    {
        return fovManager.GetBrightnessAt(transform.position);
    }

    public virtual void BeginTurn()
    {
        moveTarget = transform.position;
        takingTurn = true;
        movesRemaining = speed / 5;
        currentPath = null;
    }

    public virtual void EndTurn()
    {
        takingTurn = false;
    }

    // Use this for initialization
    protected virtual void Awake()
    {
        boardManager = GameManager.instance.boardScript;
        fovManager = GameManager.instance.fovScript;
        pathfinder = GetComponent<Pathfinder>();
        takingTurn = false;
    }
}
