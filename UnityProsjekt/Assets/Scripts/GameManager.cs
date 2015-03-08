using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public GameObject currentLevel; // Current active level gameobject
	public GameObject levelSelectPrefab; // Level item prefab for levelselect
	public GameObject[] standardLevelPrefabs; // Standard level prefabs (premade, included in game)
	public TextAsset standardCustomLevelFile; // Standard level text file (premade in the editor, included in game) Used to allow people to make levels on their own during production.
	public Transform standardLevelsParent; // UI element parent for the standard levels
	public Transform customLevelsParent; // UI element parent for the custom levels

	public AudioClip[] acWallHits; // Wall hit audiclips
	public AudioClip acBrickHit; // Brick hit audiclips

	public PlayerMode playerMode = PlayerMode.Single; // Playermode (amount of players)
	public Color[] brickColors; // Brick colors for each health level
	public Color indestructableColor; // Color for the indestrictible brick
	
	public bool gameInProgress = false; // Whether we are playing or not
	public bool isPaused = false; // Whether the game is paused or not

	private AudioSource audioSource;
	private List<GameObject> instantiatedLevelitems = new List<GameObject>(); // Instantiated levelitems
	[HideInInspector]public Vector3 topRight; // topRight.x = right edge, topRight.y = top edge
	[HideInInspector]public Vector3 bottomLeft; // bottomLeft.x = left edge, bottomLeft.y = bottom edge
	[HideInInspector]public Vector3 centerOfScreen; // Center of screen in world coordinates (in case playingfield isn't centered in the scene)
	public static GameManager instance = null;
	
	void Awake()
	{
		instance = this;
		UpdateScreenBounds ();
		audioSource = GetComponent<AudioSource>();
	}
	
	void Update()
	{
		// Pausing
		if (Input.GetButtonDown ("Pause") && gameInProgress) // Only pause if playing
		{
			if (isPaused)
			{
				UnPauseGame ();
			}
			else if (!isPaused)
			{
				PauseGame ();
			}
		}
	}

	public void SetCurrentLevel(GameObject curLevel)
	{
		currentLevel = curLevel;
	}

	/// <summary>
	/// Destroys the current levelobject.
	/// </summary>
	public void DestroyCurrentLevel()
	{
		Destroy (currentLevel);
		currentLevel = null;
	}

	/// <summary>
	/// Reloads the current level.
	/// </summary>
	public void ReloadLevel()
	{
		currentLevel.GetComponent<LevelInfo>().ReloadLevel ();
	}

	/// <summary>
	/// Displaies the levels.
	/// </summary>
	public void DisplayLevels()
	{
		foreach(GameObject obj in instantiatedLevelitems)
		{
			Destroy (obj);
		}
		instantiatedLevelitems = new List<GameObject>();

		// Standard levels
		for (int i = 0; i < standardLevelPrefabs.Length; i ++)
		{
			// Instantiate levelitem and initialize its Level
			GameObject lvlItem = Instantiate (levelSelectPrefab) as GameObject;
			lvlItem.transform.SetParent (standardLevelsParent);
			lvlItem.GetComponent<LevelSelectItem>().InitObject (standardLevelPrefabs[i]);
			instantiatedLevelitems.Add (lvlItem);
		}
		// Standard levels (from textfile)
		/* Since the data is from a spreadsheet (https://docs.google.com/spreadsheets/d/1TtFm-wD5ppWvKtOLWusXw8USFMqnKbgxbn9yumMX_Oc/edit?usp=sharing
		 * we had to remove some lines of data. */
		List<string> customPremadeLevels = new List<string>(
			standardCustomLevelFile.text.Split(new string[] {"\r\n", "\n"}, System.StringSplitOptions.None));
		customPremadeLevels.RemoveRange (0, 3); // Remove the first three lines
		List<string> levelNames = new List<string>();
		List<string> levelStrings = new List<string>();

		// Go through lines
		for (int i = 0; i < customPremadeLevels.Count; i ++)
		{
			if (i.EvenNumber ()) // If even number (see Utils.cs for function)
			{
				// Data is a level name
				levelNames.Add (customPremadeLevels[i]);
			}
			else // If odd
			{
				// Data is a level string
				levelStrings.Add (customPremadeLevels[i]);
			}
		}
		// Instantiate the custom premade levelitems
		for (int i = 0; i < levelStrings.Count; i ++)
		{
			// Instantiate levelitem and initialize its Level
			GameObject lvlItem = Instantiate (levelSelectPrefab) as GameObject;
			lvlItem.transform.SetParent (standardLevelsParent);
			lvlItem.GetComponent<LevelSelectItem>().InitObject (levelStrings[i], levelNames[i]);
			instantiatedLevelitems.Add (lvlItem);
		}

		// Custom levels
		for (int i = 0; i < LevelGenerator.instance.GetLevelIdFromPrefs (); i ++)
		{
			string levelString = LevelGenerator.instance.GetLevel (i, false); // Get the levelstring form playerprefs
			string levelName = LevelGenerator.instance.GetLevel (i, true); // Get the levelname form playerprefs
			if (levelString.Length > 0) // Make sure the level actually exists
			{
				// Instantiate levelitem and initialize its Level
				GameObject lvlItem = Instantiate (levelSelectPrefab) as GameObject;
				lvlItem.transform.SetParent (customLevelsParent);
				lvlItem.GetComponent<LevelSelectItem>().InitObject (levelString, levelName);
				instantiatedLevelitems.Add (lvlItem);
			}
		}
	}

	/// <summary>
	/// Pauses the game.
	/// </summary>
	public void PauseGame()
	{
		isPaused = true;
		GUIManager.instance.pausePanel.SetActive (true);
		gameInProgress = false;
		Time.timeScale = 0f;
		GUIManager.instance.UpdateScores ();
	}

	/// <summary>
	/// Unpauses game.
	/// </summary>
	public void UnPauseGame()
	{
		isPaused = false;
		GUIManager.instance.pausePanel.SetActive (false);
		gameInProgress = true;
		Time.timeScale = 1f;
	}

	/// <summary>
	/// Plays an audio clip.
	/// </summary>
	/// <param name="clip">Audioclip to play.</param>
	public void PlayAudioClip(AudioClip clip)
	{
		audioSource.pitch = Random.Range (0.8f, 1.2f); // Randomize the pitch to create some variance in the audio
		audioSource.PlayOneShot (clip);
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