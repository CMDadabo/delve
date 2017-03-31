using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	public int speed; // ft/sec

	protected BoardManager boardManager;
	protected Pathfinder pathfinder;

	// Use this for initialization
	protected virtual void Start () {
		boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
		pathfinder = GetComponent<Pathfinder>();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
