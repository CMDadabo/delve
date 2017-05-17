using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    public BoardManager boardScript;
    public FOVManager fovScript;
    public UIManager uiScript;
    public GameObject player;
    public GameObject playerInstance;
    public GameObject enemy;
    public GameObject logText;
    public List<GameObject> enemies;
    public bool realtime = true;

    public List<GameObject> combatOrder;
    public int activeCombatant;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        //		boardScript = GetComponent<BoardManager> ();
        InitGame();
    }

    void InitGame()
    {
        boardScript.SetupScene();

        // Create Units
        Transform units = new GameObject("Units").transform;
        playerInstance = Instantiate(player, new Vector2(5f, 5f), Quaternion.identity, units);
        enemies = new List<GameObject>();
        enemies.Add(Instantiate(enemy, new Vector2(10f, 10f), Quaternion.identity, units));
    }

    public void StartTacticalMode(List<GameObject> participants)
    {
        uiScript.AddLogMessage("Entering Tactical Mode", new Color(0, 0, 50));
        realtime = false;
        combatOrder = participants;
        activeCombatant = 0;
        combatOrder[activeCombatant].GetComponent<Unit>().BeginTurn();
    }

    public void PassTurn()
    {
        combatOrder[activeCombatant].GetComponent<Unit>().EndTurn();
        activeCombatant = (activeCombatant + 1) % combatOrder.Count;
        combatOrder[activeCombatant].GetComponent<Unit>().BeginTurn();
    }
}
