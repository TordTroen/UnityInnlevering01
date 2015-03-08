using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
	public int pregameCountdown = 3;

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

	public Text gameOverScoreText;
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
		if (playerId == 1 || playerId == 3)
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
				if (score > highest)
				{
					highest = score;
					winner = i;
				}
				pauseScoreTexts[i].text = score.ToString ();
				gameOverScoreTexts[i].text = score.ToString ();
			}
			pauseScoreTexts[i].transform.parent.gameObject.SetActive (i < playerCount);
			gameOverScoreTexts[i].transform.parent.gameObject.SetActive (i < playerCount);
		}
		gameOverScoreTexts[winner].transform.parent.gameObject.SetActive (false);
		gameOverWinnerText.text = highest.ToString ();
		gameOverWinnerText.transform.parent.GetComponent<Text>().text = "Player " + (winner + 1);
	}

	void TogglePlayerHudScores()
	{
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

	public void ToggleSound(bool active){
		float volume = 1f;
		if (active == false){
			volume = 0;
		}
		AudioListener.volume = volume;
	}

	void WipePlayingField()
	{
		GameManager.instance.WipeCurrentLevel ();
		foreach(Paddle paddle in PlayerManager.instance.allPaddles)
		{
			StartCoroutine (paddle.TerminatePaddle ());
		}
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
		string scoreText = "You got " + PlayerManager.instance.allPaddles[0].score + " points!"; // Scoretext for one player
		if (GameManager.instance.playerMode != PlayerMode.Single) // If more than one player
		{
			scoreText = "";
			for (int i = 0; i < PlayerManager.instance.allPaddles.Count; i ++)
			{
				scoreText += string.Format ("Player {0}: {1} points\n", i + 1, PlayerManager.instance.allPaddles[i].score);
			}
		}
		gameOverScoreText.text = scoreText;

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