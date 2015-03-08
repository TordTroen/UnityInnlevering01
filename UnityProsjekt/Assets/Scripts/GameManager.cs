using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public GameObject currentLevel;
	public GameObject levelSelectPrefab;
	public GameObject[] standardLevelPrefabs;
	public TextAsset standardCustomLevelFile;
	public Transform standardLevelsParent;
	public Transform customLevelsParent;
	public List<GameObject> instantiatedLevelitems = new List<GameObject>();

	public AudioClip[] acWallHits;
	public AudioClip acBrickHit;
	public bool playFirstClip = true;
	
	public PlayerMode playerMode = PlayerMode.Single; // Playermode (amount of players)
	public Color[] brickColors;
	public Color indestructableColor;
	
	public bool gameInProgress = false;
	public bool isPaused = false;

	private AudioSource audioSource;
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
		if (Input.GetButtonDown ("Pause") && gameInProgress)
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
	
	public void WipeCurrentLevel()
	{
		// TODO Call when finished Going from a level
		Destroy (currentLevel);
		currentLevel = null;
	}

	public void ReloadLevel()
	{
		currentLevel.GetComponent<LevelInfo>().ReloadLevel ();
	}

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
			GameObject lvlItem = Instantiate (levelSelectPrefab) as GameObject;
			lvlItem.transform.SetParent (standardLevelsParent);
			lvlItem.GetComponent<Level>().InitObject (standardLevelPrefabs[i]);
			instantiatedLevelitems.Add (lvlItem);
		}
		List<string> customPremadeLevels = new List<string>(
			standardCustomLevelFile.text.Split(new string[] {"\r\n", "\n"}, System.StringSplitOptions.None));
		customPremadeLevels.RemoveRange (0, 3);
		int levelCount = 0;
		List<string> levelNames = new List<string>();
		List<string> levelStrings = new List<string>();
		for (int i = 0; i < customPremadeLevels.Count; i ++)
		{
			if (i % 2 == 0)
			{
				levelNames.Add (customPremadeLevels[i]);
			}
			else
			{
				levelStrings.Add (customPremadeLevels[i]);
			}
			levelCount ++;
		}
		for (int i = 0; i < levelStrings.Count; i ++)
		{
			GameObject lvlItem = Instantiate (levelSelectPrefab) as GameObject;
			lvlItem.transform.SetParent (standardLevelsParent);
			lvlItem.GetComponent<Level>().InitObject (levelStrings[i], levelNames[i]);
			instantiatedLevelitems.Add (lvlItem);
		}
		// TODO Go through textfile and make a level for each of thos lines

		// Custom levels
		for (int i = 0; i < LevelGenerator.instance.GetLevelIdFromPrefs (); i ++)
		{
			string levelString = LevelGenerator.instance.GetLevel (i, false);
			string levelName = LevelGenerator.instance.GetLevel (i, true);
			if (levelString.Length > 0)
			{
				GameObject lvlItem = Instantiate (levelSelectPrefab) as GameObject;
				lvlItem.transform.SetParent (customLevelsParent);
				lvlItem.GetComponent<Level>().InitObject (levelString, levelName);
				instantiatedLevelitems.Add (lvlItem);
			}
		}
	}

	public void PauseGame()
	{
		isPaused = true;
		GUIManager.instance.pausePanel.SetActive (true);
		gameInProgress = false;
		Time.timeScale = 0f;
		GUIManager.instance.UpdateScores ();
	}
	
	public void UnPauseGame()
	{
		isPaused = false;
		GUIManager.instance.pausePanel.SetActive (false);
		gameInProgress = true;
		Time.timeScale = 1f;
	}

	public void PlayAudioClip(AudioClip clip)
	{
		audioSource.pitch = Random.Range (0.8f, 1.2f);
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