using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Level : MonoBehaviour 
{
	public GameObject levelPrefab;
	public Text levelNameText;

	private GameObject spawnedLevel;
	private string levelString;
	private string levelName;

	public void InitObject(GameObject lvlPrefab)
	{
		levelPrefab = lvlPrefab;
		SetName (lvlPrefab.GetComponent<LevelInfo>().levelName);
	}

	public void InitObject(string lvlString, string lvlName)
	{
		levelString = lvlString;
		SetName (lvlName);
	}

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
			spawnedLevel.GetComponent<LevelInfo>().InitializeInfo (levelString, levelName);
		}
	}

	void SetName(string newName)
	{
		levelName = newName;
		levelNameText.text = newName;
	}

	public void PlayLevel()
	{
		GUIManager.instance.LevelSelectToReady ();
		InitializeLevel ();
		GameManager.instance.SetCurrentLevel (spawnedLevel);
	}
}