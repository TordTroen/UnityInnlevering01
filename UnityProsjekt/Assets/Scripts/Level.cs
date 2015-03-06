using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Level : MonoBehaviour 
{
	public GameObject levelPrefab;
	public Text levelNameText;

	private GameObject spawnedLevel;
	private string levelString;

	public void InitObject(GameObject lvlPrefab)
	{
		levelPrefab = lvlPrefab;
		levelNameText.text = lvlPrefab.GetComponent<LevelInfo>().levelName;
	}

	public void InitObject(string lvlString, string lvlName)
	{
		levelString = lvlString;
		levelNameText.text = lvlName;
	}

	void InitializeLevel()
	{
		if (levelPrefab)
		{
			spawnedLevel = Instantiate (levelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		}
		else
		{
			spawnedLevel = LevelGenerator.instance.GenerateLevel (levelString);
		}
	}

	public void PlayLevel()
	{
		GUIManager.instance.LevelSelectToReady ();
		InitializeLevel ();
		GameManager.instance.SetCurrentLevel (spawnedLevel);
	}
}