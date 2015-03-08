using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelInfo : MonoBehaviour
{
	public string levelName = "Level";
	private GameObject levelPrefab;
	private string levelString;
	private List<GameObject> bricks = new List<GameObject>();

	/// <summary>
	/// Initializes the level.
	/// </summary>
	/// <param name="levelName">Level name.</param>
	/// <param name="levelPrefab">Level prefab.</param>
	public void InitializeInfo(string levelName, GameObject levelPrefab)
	{
		this.levelName = levelName;
		this.levelPrefab = levelPrefab;
		StartCoroutine (FindBricks ());
	}

	/// <summary>
	/// Initializes the level.
	/// </summary>
	/// <param name="levelName">Level name.</param>
	/// <param name="levelString">Level string.</param>
	public void InitializeInfo(string levelName, string levelString)
	{
		this.levelName = levelName;
		this.levelString = levelString;
		StartCoroutine (FindBricks ());
	}

	// Called when a brick is destroyed
	public void OnBrickDestroyed(GameObject destroyedBrick)
	{
		// Remove the brick and check if there are more bricks left
		bricks.Remove (destroyedBrick);
		if (bricks.Count <= 0)
		{
			// End game if all bricks are destroyed
			GUIManager.instance.PlayingToGameOver ();
		}
	}

	// Finds all bricks
	IEnumerator FindBricks()
	{
		yield return new WaitForEndOfFrame(); // Wait to end of frame so the level has properly been instantiated first

		bricks = new List<GameObject>();
		Brick[] children = GetComponentsInChildren<Brick>();
		for (int i = 0; i < children.Length; i ++)
		{
			bricks.Add (children[i].gameObject);
			children[i].levelInfoParent = this;
		}
	}

	/// <summary>
	/// Reloads the level.
	/// </summary>
	public void ReloadLevel()
	{
		GameObject lvlObj = null;
		if (levelPrefab) // If the level is a prefab
		{
			lvlObj = Instantiate (levelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			lvlObj.GetComponent<LevelInfo>().InitializeInfo (levelName, levelPrefab);
		}
		else // If the level is a string
		{
			lvlObj = LevelGenerator.instance.GenerateLevel (levelString, levelName);
			lvlObj.GetComponent<LevelInfo>().InitializeInfo (levelName, levelString);
		}
		GameManager.instance.currentLevel = lvlObj;
		Destroy (gameObject);
	}
}