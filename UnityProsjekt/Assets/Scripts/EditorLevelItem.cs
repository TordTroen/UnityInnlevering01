using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EditorLevelItem : MonoBehaviour // Level item for custom level display in the level editor
{
	public int levelId;
	public Text levelNameText;
	private LevelGenerator lvlGen;

	public void Initialize(int id, string levelName, LevelGenerator lvlGen)
	{
		levelId = id;
		levelNameText.text = levelName;
		this.lvlGen = lvlGen; // Assign here so we don't have to call GetComponent every time it gets instantiated, just assign it when initializing the levelitem
	}

	public void DeleteLevel()
	{
		lvlGen.DeleteLevelSave (levelId);
	}

	public void LoadLevel()
	{
		lvlGen.LoadToEditor (levelId);
	}
}