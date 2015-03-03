﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
	public GameObject paddlePrefab; // Paddle prefab
	public KeyCombination[] keyCombinations; // All player key combinations
	public Transform[] spawnPoints; // Spawnpoints TODO Get these dynamically from each level
	[HideInInspector]public List<Paddle> allPaddles = new List<Paddle>(); // List of all paddles in game

	public static PlayerManager instance = null;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		InitializePlayers (); // TODO put at start of round
	}
	
	void InitializePlayers()
	{
		Vector3[] spawnPositions = new Vector3[4];
		Vector3 tr = GameManager.instance.topRight;
		Vector3 bl = GameManager.instance.bottomLeft;
		Vector3 co = GameManager.instance.centerOfScreen;
		float margin = GetComponent<ScreenManager>().wallInset + 0.4f;
		spawnPositions[0] = new Vector3(co.x, bl.y + margin);
		spawnPositions[1] = new Vector3(bl.x + margin, co.y);
		spawnPositions[2] = new Vector3(co.x, tr.y - margin);
		spawnPositions[3] = new Vector3(tr.x - margin, co.y);

		for (int i = 0; i < (int)GameManager.instance.playerMode; i ++)
		{
			// Instantiate paddle and get the paddlescript
			GameObject paddleObject = Instantiate (paddlePrefab, spawnPositions[i], Quaternion.identity) as GameObject;
			Paddle paddle = paddleObject.GetComponent<Paddle>();
			
			// Add paddlescript to list and initialize the paddle
			allPaddles.Add (paddle);
			paddle.InitializePaddle (i, keyCombinations[i]);
		}
	}

	public void OnPlayerLoseHealth(int playerId, bool died)
	{
		// TODO Reset player that lost health

		// Update player health text
		GUIManager.instance.UpdatePlayerStats (playerId);

		// Check if all players are dead
		int alivePlayers = 0;
		for (int i = 0; i < allPaddles.Count; i ++)
		{
			if (allPaddles[i].IsAlive ())
			{
				alivePlayers ++;
			}
		}
		if (alivePlayers <= 0)
		{
			// TODO End game
		}
	}
}

// Class of left/right key for player initialization
// For easy assigning of player keys in the inspector
[System.Serializable]
public class KeyCombination
{
	public KeyCode leftKey = KeyCode.LeftArrow;
	public KeyCode rightKey = KeyCode.RightArrow;
}