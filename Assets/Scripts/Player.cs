using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : Unit
{

    public GameObject logLine;

    void SetLogMessage(string msg)
    {
        Destroy(logLine);
        logLine = Instantiate(GameManager.instance.logText, GameObject.Find("LogPanel").transform);
        logLine.GetComponent<Text>().text = msg;
        logLine.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        logLine.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
        logLine.GetComponent<RectTransform>().offsetMax = new Vector2(-10f, 26f);
        logLine.GetComponent<RectTransform>().offsetMin = new Vector2(10f, 10f);
    }

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
        fovManager = GameManager.instance.fovScript;
        base.Awake();
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

    // Update is called once per frame
    public void Update()
    {

        fovManager.UpdateLOS(transform.position, 5);

        // In realtime mode, let the player move freely until they detect an enemy
        if (GameManager.instance.realtime)
        {
            SetLogMessage("Realtime Mode");
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
                GameManager.instance.StartTacticalMode(combatants);
            }

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
        else
        {
            SetLogMessage("Tactical Mode");
        }

        if (!GameManager.instance.realtime && takingTurn)
            TakeTurn();

        // Follow player with camera
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -1);

    }
}
