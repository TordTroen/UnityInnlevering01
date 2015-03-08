using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
	public GameObject genBrickPrefab; // Level editor brick prefab
	public Transform gridParent; // Parent of the editor bricks
	public Transform levelViewParent; // Parent of the level display
	public GameObject levelItemPrefab; // Editor level item prefab
	public int gridWidth = 8; // Width of editorgrid
	public int gridHeight = 10; // Height of editorgrid
	public InputField levelNameInput; // Level anme inputfield

	public GameObject[] bricks; // Brick to be used for editor

	public float brickWidth = 1f; // Width of a brick
	public float brickHeight = 0.25f; // Height of a brick

	private List<GameObject> levelItems = new List<GameObject>();
	private List<Transform> genBricks = new List<Transform>();
	private int toolId; // Current tool
	private string levelString;
	private string levelName;
	private string levelSaveKey = "customlevel_"; // Key for saving level to playerprefs
	private string curLevelIdKey = "levelId_"; // Key for saving levelid to playerprefs
	private int curLevelSaveId = 0; // Level id for saving (so its easy to loop through)

	public static LevelGenerator instance;

	void Awake()
	{
		instance = this;
		curLevelSaveId = GetLevelIdFromPrefs ();
	}

	public void InitializeLevelEditor()
	{
		// Instantiate the editor items (only if the gird isn't already instantiated)
		if (genBricks.Count < gridWidth * gridHeight)
		{
			foreach(Transform t in genBricks)
			{
				Destroy (t.gameObject);
			}
			genBricks = new List<Transform>();
			for (int i = 0; i < gridWidth * gridHeight; i ++)
			{
				GameObject obj = Instantiate (genBrickPrefab) as GameObject;
				obj.transform.SetParent (gridParent);
				genBricks.Add (obj.transform);
				obj.GetComponent<LevelToolButton>().id = i;
			}
		}

		// Display all saved levels
		DisplaySavedLevels ();
		
		// Reset the editor
		UpdateEditorGrid ();
		SelectTool (0);
		ResetLevelString ();
		ClearEditor ();
	}

	/// <summary>
	/// Updates the size of the grid to fit the screen)
	/// </summary>
	void UpdateEditorGrid()
	{
		float space = 3f;
		float width = ((RectTransform)gridParent).rect.width / gridWidth - space;
		GridLayoutGroup glg = gridParent.GetComponent<GridLayoutGroup>();
		glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		glg.constraintCount = gridWidth;
		glg.cellSize = new Vector2(width, width * 0.25f);  //.GetComponent<RectTransform>()
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

	/// <summary>
	/// Generates a level.
	/// </summary>
	/// <returns>The level.</returns>
	/// <param name="lvlString">Lvl string.</param>
	/// <param name="name">Name.</param>
	public GameObject GenerateLevel(string lvlString, string name)
	{
		int i = 0;
		float xPos = -(brickWidth + (brickWidth * gridWidth)) * 0.5f + brickWidth;
		float yPos = (brickHeight + (brickHeight * gridHeight)) * 0.5f - brickHeight;
		Vector3 pos = new Vector3(xPos, yPos);
		Transform lvlParent = new GameObject("Level_" + name, typeof(LevelInfo)).transform;
		lvlParent.GetComponent<LevelInfo>().InitializeInfo (name, lvlString);

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
		SaveLevelToPrefs (levelString, levelName);

		// Display all levels
		DisplaySavedLevels ();
	}

	void SaveLevelToPrefs(string lvlString, string lvlName)
	{
		// Make sure the name is valid
		if (!ValidName ())
		{
			// Notify the player and stop the saving
			// TODO Notify the player
			Debug.LogError ("INVALID NAME!");
			return;
		}
		
		// Save level with name to playerprefs
		PlayerPrefs.SetString (levelSaveKey + curLevelSaveId, lvlName + "\n" + lvlString);
		// Increment id save WARNING: must only be done from here, or else problems will occur
		curLevelSaveId ++;
		// Save the new levelId
		PlayerPrefs.SetInt (curLevelIdKey, curLevelSaveId);
		
		// Make sure Unity saves the prefs to file in case a crash before Unity does it automatically
		PlayerPrefs.Save ();
	}

	string GetLevelPref(int id)
	{
		return PlayerPrefs.GetString (levelSaveKey + id);
	}
	
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
		if (!PlayerPrefs.HasKey (levelSaveKey + id))
		{
			return "";
		}
		return GetLevelPref (id).Split(new string[] {"\r\n", "\n"}, System.StringSplitOptions.None)[lineNumber];
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
			string lvl = GetLevel (i, true);
			if (lvl.Length > 0)
			{
				GameObject obj = Instantiate (levelItemPrefab) as GameObject;
				obj.transform.SetParent (levelViewParent);
				obj.GetComponent<EditorLevelItem>().Initialize (i, lvl, this);
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
}
