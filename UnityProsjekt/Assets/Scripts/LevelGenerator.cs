using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
	public GameObject genBrickPrefab;
	public Transform gridParent;
	public Transform levelViewParent;
	public GameObject levelItemPrefab;
	public List<GameObject> levelItems = new List<GameObject>(); // TODO Private
	public List<Transform> genBricks = new List<Transform>(); // TODO Private
	public int toolId; // TODO Private
	public string levelString; // TODO Private
	public int gridWidth = 8;
	public int gridHeight = 10;
	public string levelName; // TODO Private
	public InputField levelNameInput;

	public GameObject[] bricks;

	public float brickWidth = 1f;
	public float brickHeight = 0.25f;

	private string levelSaveKey = "customlevel_";
	private string curLevelIdKey = "levelId_";
	private int curLevelSaveId = 0;

	public static LevelGenerator instance;

	void Awake()
	{
		instance = this;
		curLevelSaveId = GetLevelIdFromPrefs ();
	}

	public void InitializeLevelEditor()
	{
		// TODO Call this when going to the levelgen menu, not in start
		//genBricks = new List<Transform>(gridParent.GetComponentsInChildren<Transform>());
		//genBricks.RemoveAt (0);
		foreach(Transform t in genBricks)
		{
			Destroy (t);
		}
		genBricks = new List<Transform>();
		for (int i = 0; i < gridWidth * gridHeight; i ++)
		{
			GameObject obj = Instantiate (genBrickPrefab) as GameObject;
			obj.transform.SetParent (gridParent);
			genBricks.Add (obj.transform);
			obj.GetComponent<LevelToolButton>().id = i;
		}
		float space = 3f;
		float width = ((RectTransform)gridParent).rect.width / gridWidth - space;
		GridLayoutGroup glg = gridParent.GetComponent<GridLayoutGroup>();
		glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		glg.constraintCount = gridWidth;
		glg.cellSize = new Vector2(width, width * 0.25f);  //.GetComponent<RectTransform>()

		ResetLevelString ();
		
		// Display all saved levels
		DisplaySavedLevels ();
		
		// Select the first tool
		SelectTool (0);
	}

	/// <summary>
	/// Resets the level string (set everything to empty bricks).
	/// </summary>
	void ResetLevelString()
	{
		levelString = "";
		for (int i = 0; i < gridWidth * gridHeight; i ++)
		{
			levelString += "0";
		}
	}

	public void SelectTool(int id)
	{
		// Sets the tool ID
		toolId = id;
	}

	public void SetBrick(LevelToolButton button)
	{
		// Set visual color of button to correct brick color
		UpdateBrickColor (button.img, toolId);

		// Add correct brick id to levelstring
		levelString = levelString.Remove (button.id, 1);
		levelString = levelString.Insert (button.id, toolId.ToString ());
	}

	void UpdateBrickColor(Image image, int colorId)
	{
		// Set visual color of button to correct brick color
		Color brickColor = GameManager.instance.indestructableColor;
		if (colorId == 0)
		{
			brickColor = Color.white;
		}
		else if (colorId < 4)
		{
			brickColor = GameManager.instance.brickColors[colorId];
		}
		image.color = brickColor;
	}

	public void GenerateLevel()
	{
		GenerateLevel (levelString, levelName); // TODO Remove
	}

	public GameObject GenerateLevel(string lvlString, string name)
	{
		int i = 0;
		float xPos = -(brickWidth + (brickWidth * gridWidth) * 0.5f);
		float yPos = (brickHeight + (brickHeight * gridHeight)) * 0.5f;
		Vector3 pos = new Vector3(xPos, yPos);
		Transform lvlParent = new GameObject("Level_" + name, typeof(LevelInfo)).transform;
		lvlParent.GetComponent<LevelInfo>().InitializeInfo (lvlString, name);

		// Iterate through the rows
		for (int y = 0; y < gridHeight; y ++)
		{
			// Iterate through each brick on this row
			for (int x = 0; x < gridWidth; x ++)
			{
				// Convert the character to an int
				int id = CharToId (lvlString[i]);
				
				// If there is a brick in the array (the first is empty, because the first brick is empty...)
				if (bricks[id])
				{
					// Instantiate at pos
					GameObject obj = Instantiate (bricks[id], pos, Quaternion.identity) as GameObject;
					obj.transform.SetParent (lvlParent);
				}
				// Add the brickWidth to the x pos
				pos.x += brickWidth;
				i ++;
			}
			// Start from the left again on the x axis
			pos.x = xPos;
			// Add the brickHeigth to the y pos
			pos.y -= brickHeight;
		}

		return lvlParent.gameObject;
	}

	int CharToId(char c)
	{
		// Get the characters value and subtract by 48 (to map char 0 to value 0)
		int val = c - 48;

		// Return -1 if no a number character
		if (val < 0 || val > 9)
		{
			return -1;
		}

		return val;
	}

	public void SetLevelName(string text)
	{
		levelName = text;
	}

	public void SaveLevel()
	{
		// Make sure the name is valid
		if (!ValidName ())
		{
			// Notify the player and stop the saving
			// TODO Notify the player
			print ("INVALID NAME!");
			return;
		}

		// Save level with name to playerprefs
		PlayerPrefs.SetString (levelSaveKey + curLevelSaveId, levelName + "\n" + levelString);
		// Increment id save WARNING: must only be done from here, or else problems will occur
		curLevelSaveId ++;
		// Save the new levelId
		PlayerPrefs.SetInt (curLevelIdKey, curLevelSaveId);

		// Make sure Unity saves the prefs to file in case a crash before Unity does it automatically
		PlayerPrefs.Save ();

		// Display all levels
		DisplaySavedLevels ();
	}

	string GetLevelPref(int id)
	{
		return PlayerPrefs.GetString (levelSaveKey + id);
	}

	/*string GetLevelName(int id)
	{
		return GetLevelPref (id).Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None)[0];
	}

	string GetLevelString(int id)
	{
		return GetLevelPref (id).Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None)[1];
	}*/

	/// <summary>
	/// Returns the level pref (either name or levelstring) of id. 
	/// </summary>
	/// <returns>The level.</returns>
	/// <param name="id">Level id.</param>
	/// <param name="name">If set to <c>true</c> returns the name, otherwise returns the levelstring.</param>
	public string GetLevel(int id, bool name)
	{
		int lineNumber = 0;
		if (!name)
		{
			lineNumber = 1;
		}
		return GetLevelPref (id).Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None)[lineNumber];
	}

	void DisplaySavedLevels()
	{
		for (int i = 0; i < levelItems.Count; i ++)
		{
			Destroy (levelItems[i]);
		}
		levelItems = new List<GameObject>();

		for (int i = 0; i < curLevelSaveId; i ++)
		{
			if (GetLevel (i, true).Length > 0)
			{
				GameObject obj = Instantiate (levelItemPrefab) as GameObject;
				obj.transform.SetParent (levelViewParent);
				obj.GetComponent<LevelItem>().Initialize (i, GetLevel (i, true), this);
				levelItems.Add (obj);
			}
		}
	}

	public int GetLevelIdFromPrefs()
	{
		return PlayerPrefs.GetInt (curLevelIdKey);
	}

	public void DeleteLevelSave(int id)
	{
		// TODO Resave levels with new id
		/* 1. Put all saves in a list
		 * 2. Delete level
		 * 3. Reset id counter
		 * 4. Resave all levels
		 * That way when deleting a level, the id gets free'd up so it doesn't take up an id (even though the chances of saving over 2b. levels are pretty low...)
		 */
		PlayerPrefs.DeleteKey (levelSaveKey + id);
		DisplaySavedLevels ();
	}

	public void LoadToEditor(int id)
	{
		// Get leveltring and name
		string lvl = GetLevel (id, false);
		string name = GetLevel (id, true);

		// Update bricks
		for (int i = 0; i < lvl.Length; i ++)
		{
			UpdateBrickColor (genBricks[i].GetComponent<LevelToolButton>().img, CharToId (lvl[i]));
		}

		// Set levelstring and input to loaded string
		levelString = lvl;
		levelNameInput.text = name;
	}

	public void ClearEditor()
	{
		// Clear levelname and name input
		levelName = "";
		levelNameInput.text = "";
		// Clear levelstring
		ResetLevelString ();

		// Clear editorbricks
		for (int i = 0; i < levelString.Length; i ++)
		{
			UpdateBrickColor (genBricks[i].GetComponent<LevelToolButton>().img, CharToId (levelString[i]));
		}
	}

	bool ValidName()
	{
		// Check if levelname is null or empty
		if (string.IsNullOrEmpty (levelName))
		{
			return false;
		}

		// Check if levelname contains something other than whitespaces
		for (int i = 0; i < levelName.Length; i ++)
		{
			if (levelName[i] != ' ')
			{
				return true;
			}
		}
		return false;
	}

	public void DeleteAllLevels()
	{
		// TODO Notify player first
		// Delete all keys
		PlayerPrefs.DeleteAll ();

		// Reset level save id
		PlayerPrefs.SetInt (curLevelIdKey, 0);
		curLevelSaveId = GetLevelIdFromPrefs ();

		PlayerPrefs.Save ();

		DisplaySavedLevels ();
	}

	/*
	 * 1. Turn GO grid into string
	 * 2. Save string
	 * 3. Load string
	 * 4. Convert string to grid
	 */
}
