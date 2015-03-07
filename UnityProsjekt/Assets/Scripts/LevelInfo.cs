using UnityEngine;
using System.Collections;

public class LevelInfo : MonoBehaviour
{
	public string levelName = "Level";
	private GameObject levelPrefab;
	private string levelString;

	public void InitializeInfo(string levelName, GameObject levelPrefab)
	{
		this.levelName = levelName;
		this.levelPrefab = levelPrefab;
	}

	public void InitializeInfo(string levelName, string levelString)
	{
		this.levelName = levelName;
		this.levelString = levelString;
	}

	public void ReloadLevel()
	{
		GameObject lvlObj = null;
		if (levelPrefab)
		{
			lvlObj = Instantiate (levelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			lvlObj.GetComponent<LevelInfo>().InitializeInfo (levelName, levelPrefab);
		}
		else
		{
			lvlObj = LevelGenerator.instance.GenerateLevel (levelString, levelName);
		}
		GameManager.instance.currentLevel = lvlObj;
		Destroy (gameObject);
	}
}