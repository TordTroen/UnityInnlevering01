using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelInfo : MonoBehaviour
{
	public string levelName = "Level";
	public GameObject levelPrefab;
	public string levelString;
	public List<GameObject> bricks = new List<GameObject>();

	public void InitializeInfo(string levelName, GameObject levelPrefab)
	{
		this.levelName = levelName;
		this.levelPrefab = levelPrefab;
		StartCoroutine (FindBricks ());
	}

	public void InitializeInfo(string levelName, string levelString)
	{
		this.levelName = levelName;
		this.levelString = levelString;
		StartCoroutine (FindBricks ());
	}

	public void OnBrickDestroyed(GameObject destroyedBrick)
	{
		bricks.Remove (destroyedBrick);
		if (bricks.Count <= 0)
		{
			GUIManager.instance.PlayingToGameOver ();
		}
	}

	IEnumerator FindBricks()
	{
		yield return new WaitForEndOfFrame();

		bricks = new List<GameObject>();
		Brick[] children = GetComponentsInChildren<Brick>();
		for (int i = 0; i < children.Length; i ++)
		{
			bricks.Add (children[i].gameObject);
			children[i].levelInfoParent = this;
		}
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
			lvlObj.GetComponent<LevelInfo>().InitializeInfo (levelName, levelString);
		}
		GameManager.instance.currentLevel = lvlObj;
		Destroy (gameObject);
	}
}