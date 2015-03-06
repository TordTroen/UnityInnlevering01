﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public bool playingClassic;
	public AudioClip audioClipWallHit1;
	public AudioClip audioClipWallHit2;
	public AudioClip audioClipBrickHit;
	public bool playFirstClip = true;
	
	public PlayerMode playerMode = PlayerMode.Single; // Playermode (amount of players)
	public Color[] brickColors;
	public Color indestructableColor;
	
	public bool gameInProgress = false;
	public bool isPaused = false;
	
	[HideInInspector]public Vector3 topRight; // topRight.x = right edge, topRight.y = top edge
	[HideInInspector]public Vector3 bottomLeft; // bottomLeft.x = left edge, bottomLeft.y = bottom edge
	[HideInInspector]public Vector3 centerOfScreen; // Center of screen in world coordinates (in case playingfield isn't centered in the scene)
	public static GameManager instance = null;
	
	void Awake()
	{
		instance = this;
		UpdateScreenBounds ();
		if(GameObject.Find("levelClassic").activeInHierarchy){
			playingClassic = true;
		}
	}
	
	void Update()
	{
		if (Input.GetButtonDown ("Pause"))
		{
			if (isPaused)
			{
				UnPauseGame ();
			}
			else if (!isPaused && gameInProgress)
			{
				PauseGame ();
			}
		}
	}
	
	public void PauseGame()
	{
		isPaused = true;
		GUIManager.instance.pausePanel.SetActive (true);
		gameInProgress = false;
		Time.timeScale = 0f;
	}
	
	public void UnPauseGame()
	{
		isPaused = false;
		GUIManager.instance.pausePanel.SetActive (false);
		gameInProgress = true;
		Time.timeScale = 1f;
	}
	
	public void MainMenuToReadyGame()
	{
		// UI Activate/deactivate
		GUIManager.instance.mainPanel.SetActive (false);
		GUIManager.instance.readyPanel.SetActive (true);
		
		// Calls
		PlayerManager.instance.InitializePlayers ();
		StartCoroutine (GUIManager.instance.DoCountdown ());
	}
	
	public void ReadyGameToPlaying()
	{
		// UI Activate/deactivate
		GUIManager.instance.readyPanel.SetActive (false);
		
		// Calls
		gameInProgress = true;
		PlayerManager.instance.StartPlayers ();
	}
	
	public void PlayingToGameOver()
	{
		// UI Activate/deactivate
		GUIManager.instance.gameOverPanel.SetActive (true);
		
		// Calls
		gameInProgress = false;
		string scoreText = "You got " + PlayerManager.instance.allPaddles[0].score + " points!"; // Scoretext for one player
		if (playerMode != PlayerMode.Single) // If more than one player
		{
			scoreText = "";
			for (int i = 0; i < PlayerManager.instance.allPaddles.Count; i ++)
			{
				scoreText += string.Format ("Player {0}: {1} points\n", i + 1, PlayerManager.instance.allPaddles[i].score);
			}
		}
		GUIManager.instance.gameOverScoreText.text = scoreText;
	}
	
	public void GameOverToReadyGame()
	{
		// UI Activate/deactivate
		GUIManager.instance.gameOverPanel.SetActive (false);
		
		// Calls
		MainMenuToReadyGame ();
	}
	
	public void GameOverToMainMenu()
	{
		// UI Activate/deactivate
		GUIManager.instance.gameOverPanel.SetActive (false);
		GUIManager.instance.mainPanel.SetActive (true);
		
		// Calls
	}
	
	/// <summary>
	/// Updates the screen bounds.
	/// </summary>
	public void UpdateScreenBounds()
	{
		Camera cam = Camera.main;
		bottomLeft = cam.ScreenToWorldPoint (new Vector3 (0f, 0f)); // Bottomleft corner from screen to world coordinates
		topRight = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth, cam.pixelHeight)); // Topright corner from screen to world coordinates
		centerOfScreen = cam.ScreenToWorldPoint (new Vector3(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f)); // Center of screen to world coordinates
		bottomLeft.z = 0f; // Zero out the z
		topRight.z = 0f; // Zero out the z
		centerOfScreen.z = 0f; // Zero out the z
	}
}

public enum PlayerMode
{
	Single = 1,
	Two = 2,
	Three = 3,
	Four = 4
}