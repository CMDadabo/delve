using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public BoardManager boardScript;
	public FOVManager fovScript;
	public GameObject player;

	private bool realtime = true;

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad(gameObject);
//		boardScript = GetComponent<BoardManager> ();
		InitGame ();
	}

	void InitGame ()
	{
		boardScript.SetupScene ();
		Instantiate(player, new Vector2( 5f, 5f ), Quaternion.identity, new GameObject ("Units").transform );
	}

	// Update is called once per frame
	void Update ()
	{

	}
}
