using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelectItem : MonoBehaviour // For the levelitems in levelselect
{
	public Text levelNameText; // Levelname textitem

	[HideInInspector]public GameObject levelPrefab; // Level prefab (when not a custom level)
	private GameObject spawnedLevel;
	private string levelString;
	private string levelName;

	/// <summary>
	/// Initializes the object with a prefab.
	/// </summary>
	/// <param name="lvlPrefab">Lvl prefab.</param>
	public void InitObject(GameObject lvlPrefab)
	{
		levelPrefab = lvlPrefab;
		SetName (lvlPrefab.GetComponent<LevelInfo>().levelName);
	}

	/// <summary>
	/// /// Initializes the object with a string.
	/// </summary>
	/// <param name="lvlString">Lvl string.</param>
	/// <param name="lvlName">Lvl name.</param>
	public void InitObject(string lvlString, string lvlName)
	{
		levelString = lvlString;
		SetName (lvlName);
	}

	/// <summary>
	/// Initializes the level and ready it for playing.
	/// </summary>
	void InitializeLevel()
	{
		if (levelPrefab)
		{
			spawnedLevel = Instantiate (levelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			spawnedLevel.GetComponent<LevelInfo>().InitializeInfo (levelName, levelPrefab);
		}
		else
		{
			spawnedLevel = LevelGenerator.instance.GenerateLevel (levelString, levelName);
			spawnedLevel.GetComponent<LevelInfo>().InitializeInfo (levelName, levelString);
		}
	}

	void SetName(string newName)
	{
		levelName = newName;
		levelNameText.text = newName;
	}

	/// <summary>
	/// Starts the current level
	/// </summary>
	public void PlayLevel()
	{
		GUIManager.instance.LevelSelectToReady ();
		InitializeLevel ();
		GameManager.instance.SetCurrentLevel (spawnedLevel);
	}
}