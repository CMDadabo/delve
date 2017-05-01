using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	public int speed; // ft/sec

	protected Pathfinder pathfinder;
	protected BoardManager boardManager;

	// Use this for initialization
	protected virtual void Awake () {
		boardManager = GameManager.instance.boardScript;
		pathfinder = GetComponent<Pathfinder>();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
