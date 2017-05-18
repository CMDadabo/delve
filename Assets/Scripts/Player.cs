using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{

    public GameObject logLine;

    void OnDrawGizmos()
    {
        // Color path when it exists
        if (currentPath != null)
        {
            Gizmos.color = new Color(0.85f, 0.75f, 0f, 0.25f);
            currentPath.ForEach(vector => Gizmos.DrawCube(vector, new Vector3(1, 1, 1)));
        }
    }

    protected override void Awake()
    {
        // Set stats
        stats.hp = 20;

        fovManager = GameManager.instance.fovScript;
        uiManager = GameManager.instance.uiScript;
        base.Awake();
    }

    public override void BeginTurn()
    {
        base.BeginTurn();
        uiManager.AddLogMessage("It's your turn to move.", Color.black);
    }

    protected void TakeTurn()
    {

        if ((Vector2)transform.position != moveTarget)
        {
            transform.position = Vector2.MoveTowards(transform.position, moveTarget, 10f * Time.deltaTime);
        }
        else if (movesRemaining == 0)
        {
            GameManager.instance.PassTurn();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveTarget = (Vector2)transform.position + Vector2.right;
            movesRemaining--;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveTarget = (Vector2)transform.position + Vector2.down;
            movesRemaining--;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveTarget = (Vector2)transform.position + Vector2.left;
            movesRemaining--;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            moveTarget = (Vector2)transform.position + Vector2.up;
            movesRemaining--;
        }
    }

    protected void HandleRealtimePlayerMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 currentCoords = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
            currentPath = pathfinder.FindPath(currentCoords, boardManager.SnapMouseToGrid());
            moveTarget = currentPath[currentPath.Count - 1];
            currentPath.RemoveAt(currentPath.Count - 1);
        }

        if (currentPath != null)
        {
            if ((Vector2)transform.position != moveTarget)
            {
                transform.position = Vector2.MoveTowards(transform.position, moveTarget, 10f * Time.deltaTime);
            }
            else if (currentPath.Count > 0)
            {
                moveTarget = currentPath[currentPath.Count - 1];
                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }
    }

    protected void CheckForDetectedEnemies()
    {
        List<GameObject> allEnemies = GameManager.instance.enemies;
        List<GameObject> combatants = new List<GameObject>();
        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (Sees(allEnemies[i].GetComponent<Unit>()))
            {
                combatants.Add(allEnemies[i]);
            }
        }
        if (combatants.Count > 0)
        {
            combatants.Add(gameObject);
            // Move the player to their target square immediately so they don't freeze in between squares
            transform.position = Vector2.MoveTowards(transform.position, moveTarget, 1f);
            GameManager.instance.StartTacticalMode(combatants);
        }
    }

    // Update is called once per frame
    public void Update()
    {

        fovManager.UpdateLOS(transform.position, 5);

        // In realtime mode, let the player move freely until they detect an enemy
        if (GameManager.instance.realtime)
        {
            CheckForDetectedEnemies();
            HandleRealtimePlayerMovement();
        }

        if (!GameManager.instance.realtime && takingTurn)
            TakeTurn();

        // Follow player with camera
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -1);

    }
}
