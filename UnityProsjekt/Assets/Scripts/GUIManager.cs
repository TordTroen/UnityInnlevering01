using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
	public int pregameCountdown = 3; // Number of seconds for the pregame countdown

	// All UI panels
	public GameObject mainPanel;
	public GameObject levelSelectPanel;
	public GameObject levelEditorPanel;
	public GameObject settingsPanel;
	public GameObject readyPanel;
	public GameObject pausePanel;
	public GameObject gameOverPanel;
	public GameObject hudScorePanel;

	public Button classicLevelButton;
	public Text classicLevelText;

	// All score texts
	public Text countdownText;
	public Text[] scoreTexts;
	public Text[] pauseScoreTexts;
	public Text[] gameOverScoreTexts;
	public Text gameOverWinnerText;

	public Text playerModeText;

	private int playerNumber = 1;
	private ScreenManager screenManager;
	public static GUIManager instance;

	void Awake()
	{
		instance = this;

		screenManager = GetComponent<ScreenManager>();
	}

	void Start()
	{
		TogglePlayerHudScores ();
		hudScorePanel.SetActive (false);
	}

	public void UpdatePlayerStats(int playerId)
	{
		Paddle player = PlayerManager.instance.allPaddles[playerId];
		string divider = " - ";
		if (playerId == 1 || playerId == 3) // Divide with a linebreak if on the sides
		{
			divider = "\n";
		}
		scoreTexts[playerId].text = string.Format ("<color=#22df71>{0}{1}</color><color=#26a2df>{2}</color>", player.curHealth, divider, player.score.ToString ());
	}

	public void UpdateScores()
	{
		int winner = 0;
		int highest = -1;
		int playerCount = (int)GameManager.instance.playerMode;

		for (int i = 0; i < pauseScoreTexts.Length; i ++)
		{
			if (i < playerCount)
			{
				int score = PlayerManager.instance.allPaddles[i].score;
				// Find highest score
				if (score > highest)
				{
					highest = score;
					winner = i;
				}
				// Set scores
				pauseScoreTexts[i].text = score.ToString ();
				gameOverScoreTexts[i].text = score.ToString ();
			}
			// Toggle score objects depening on playercount
			pauseScoreTexts[i].transform.parent.gameObject.SetActive (i < playerCount);
			gameOverScoreTexts[i].transform.parent.gameObject.SetActive (i < playerCount);
		}
		// Toggle and assign the winner
		gameOverScoreTexts[winner].transform.parent.gameObject.SetActive (false);
		gameOverWinnerText.text = highest.ToString ();
		gameOverWinnerText.transform.parent.GetComponent<Text>().text = "Player " + (winner + 1);
	}

	void TogglePlayerHudScores()
	{
		// Toggle the HUD scores depening on the playercount
		for (int i = 0; i < scoreTexts.Length; i ++)
		{
			scoreTexts[i].gameObject.SetActive (i < (int)GameManager.instance.playerMode);
		}
	}

	public IEnumerator DoCountdown()
	{
		// Toggle readyPanel
		readyPanel.SetActive (GameManager.instance.playerMode != PlayerMode.Single);

		if (GameManager.instance.playerMode != PlayerMode.Single) // If multiplayer
		{
			// Countdown
			for (int i = pregameCountdown; i > 0; i --)
			{
				countdownText.text = i.ToString ();
				yield return new WaitForSeconds(1f);
			}
		}
		else
		{
			// Must wait so everything initializes
			yield return new WaitForEndOfFrame();
		}
		// Start game
		ReadyGameToPlaying ();
	}

	/// <summary>
	/// Wipes the playing field.
	/// </summary>
	void WipePlayingField()
	{
		GameManager.instance.DestroyCurrentLevel ();
		foreach(Paddle paddle in PlayerManager.instance.allPaddles)
		{
			StartCoroutine (paddle.TerminatePaddle ());
		}
	}

	/////////////////////////////////
	// Begin all the GUI functions //
	/////////////////////////////////
	
	public void QuitApplication()
	{
		Application.Quit ();
	}

	/// <summary>
	/// Changes the player mode.
	/// </summary>
	/// <param name="next">If set to <c>true</c> next playermode.</param>
	public void ChangePlayerMode(bool next) {
		playerNumber = playerNumber.IncrementDecrement (1, 5, next, true);
		GameManager.instance.playerMode = (PlayerMode)playerNumber;
		string playerString = GameManager.instance.playerMode.ToString().ToUpper () + "PLAYER";
		if (playerNumber != 1) {
			playerString = playerNumber + " PLAYERS";
		}
		playerModeText.text = playerString;
		
		TogglePlayerHudScores ();
		
		screenManager.ArrangeWalls ();
	}
	
	/// <summary>
	/// Toggles the sound.
	/// </summary>
	/// <param name="active">If set to <c>true</c> active.</param>
	public void ToggleSound(bool active){
		float volume = 1f;
		if (active == false){
			volume = 0;
		}
		AudioListener.volume = volume;
	}

	public void MainMenuToReadyGame()
	{
		// UI Activate/deactivate
		mainPanel.SetActive (false);
		readyPanel.SetActive (true);
		
		// Calls
		PlayerManager.instance.InitializePlayers ();
		StartCoroutine (DoCountdown ());
	}

	public void MainMenuToLevelSelect()
	{
		// UI Activate/deactivate
		mainPanel.SetActive (false);
		levelSelectPanel.SetActive (true);
		
		// Calls
		// Make sure the classic level can only be played in singleplayer
		classicLevelButton.interactable = GameManager.instance.playerMode == PlayerMode.Single;
		string buttonText = "PLAY";
		if (!classicLevelButton.interactable)
		{
			buttonText = "Singleplayer only";
		}
		classicLevelText.text = buttonText;
		
		// Display all levels
		GameManager.instance.DisplayLevels ();
	}

	public void LevelSelectToReady()
	{
		// UI Activate/deactivate
		levelSelectPanel.SetActive (false);
		readyPanel.SetActive (true);

		// Calls
		PlayerManager.instance.InitializePlayers ();
		StartCoroutine (DoCountdown ());
	}

	public void ReadyGameToPlaying()
	{
		// UI Activate/deactivate
		readyPanel.SetActive (false);
		hudScorePanel.SetActive (true);

		// Calls
		GameManager.instance.gameInProgress = true;
		PlayerManager.instance.StartPlayers ();
	}
	
	public void PlayingToGameOver()
	{
		// UI Activate/deactivate
		gameOverPanel.SetActive (true);
		hudScorePanel.SetActive (false);

		// Calls
		UpdateScores ();
		GameManager.instance.gameInProgress = false;

		foreach(Paddle player in PlayerManager.instance.allPaddles)
		{
			player.ball.gameObject.SetActive (false);
		}
	}
	
	public void GameOverToReadyGame()
	{
		// UI Activate/deactivate
		gameOverPanel.SetActive (false);
		
		// Calls
		MainMenuToReadyGame ();
	}

	public void GameOverToLevelSelect()
	{
		// UI Activate/deactivate
		
		// Calls
		GameOverToMainMenu ();
		MainMenuToLevelSelect ();
	}

	public void GameOverToMainMenu()
	{
		// UI Activate/deactivate
		gameOverPanel.SetActive (false);
		hudScorePanel.SetActive (false);
		mainPanel.SetActive (true);
		
		// Calls
		WipePlayingField ();
	}
	
	public void MainMenuToSettings()
	{
		// UI Activate/deactivate
		mainPanel.SetActive (false);
		settingsPanel.SetActive (true);
		
		// Calls
	}
	
	public void SettingsToMainMenu()
	{
		// UI Activate/deactivate
		settingsPanel.SetActive (false);
		mainPanel.SetActive (true);
		
		// Calls
	}
	
	public void LevelSelectToMainMenu()
	{
		// UI Activate/deactivate
		levelSelectPanel.SetActive (false);
		mainPanel.SetActive (true);
		
		// Calls
	}
	
	public void MainMenuToLevelEditor()
	{
		// UI Activate/deactivate
		mainPanel.SetActive (false);
		levelEditorPanel.SetActive (true);
		
		// Calls
		LevelGenerator.instance.InitializeLevelEditor ();
	}
	
	public void LevelEditorToMainMenu()
	{
		// UI Activate/deactivate
		levelEditorPanel.SetActive (false);
		mainPanel.SetActive (true);
		
		// Calls
	}

	public void PauseRestart()
	{
		// UI Activate/deactivate

		// Calls
		GameManager.instance.UnPauseGame ();
		PlayingToGameOver ();
		GameOverToReadyGame ();
		// TODO Reload level
		GameManager.instance.ReloadLevel ();
	}
}