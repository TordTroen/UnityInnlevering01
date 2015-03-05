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
	public List<GameObject> levelItems = new List<GameObject>();
	public List<Transform> genBricks = new List<Transform>();
	public int toolId;
	public string levelString;
	public int gridWidth = 8; // TODO define min/max and allow player to change
	public int gridHeight = 10;
	public string levelName;

	public GameObject[] bricks;

	public InputField input;
	public float brickWidth = 0.5f;
	public float brickHeight = 0.25f;
	public string prevText;

	private string levelSaveKey = "customlevel_";
	private string curLevelIdKey = "levelId_";
	private int curLevelSaveId = 0;

	void Awake()
	{
		curLevelSaveId = GetLevelIdFromPrefs ();
	}

	void Start()
	{
		//genBricks = new List<Transform>(gridParent.GetComponentsInChildren<Transform>());
		//genBricks.RemoveAt (0);

		for (int i = 0; i < gridWidth * gridHeight; i ++)
		{
			GameObject obj = Instantiate (genBrickPrefab) as GameObject;
			obj.transform.SetParent (gridParent);
			genBricks.Add (obj.transform);
			obj.GetComponent<LevelToolButton>().id = i;
			levelString += "0";
		}

		DisplaySavedLevels ();
	}

	public void SelectTool(int id)
	{
		// Sets the tool ID
		toolId = id;
	}

	public void SetBrick(LevelToolButton button)
	{
		// Set visual color of button to correct brick color
		Color brickColor = GameManager.instance.indestructableColor;
		if (toolId == 0)
		{
			brickColor = Color.white;
		}
		else if (toolId < 4)
		{
			brickColor = GameManager.instance.brickColors[toolId];
		}
		button.img.color = brickColor;

		// Add correct brick id to levelstring
		levelString = levelString.Remove (button.id, 1);
		levelString = levelString.Insert (button.id, toolId.ToString ());
	}

	public void GenerateLevel()
	{
		int i = 0;
		Vector3 pos = Vector3.zero;

		// Iterate through the rows
		for (int y = 0; y < gridHeight; y ++)
		{
			// Iterate through each brick on this row
			for (int x = 0; x < gridWidth; x ++)
			{
				// Convert the character to an int
				int id = CharToId (levelString[i]);

				// If there is a brick in the array (the first is empty, because the first brick is empty...)
				if (bricks[id])
				{
					// Instantiate at pos
					Instantiate (bricks[id], pos, Quaternion.identity);
				}
				// Add the brickWidth to the x pos
				pos.x += brickWidth;
				i ++;
			}
			// Start from the left again on the x axis
			pos.x = 0f;
			// Add the brickHeigth to the y pos
			pos.y -= brickHeight;
		}
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
		// Save level with name to playerprefs
		PlayerPrefs.SetString (levelSaveKey + curLevelSaveId, levelName + "\n" + levelString);
		// Increment id save WARNING: must only be done from here, or else problems will occur
		curLevelSaveId ++;
		// Save the new levelId
		PlayerPrefs.SetInt (curLevelIdKey, curLevelSaveId);

		PlayerPrefs.Save ();

		DisplaySavedLevels ();
	}

	public void ListAllLevels()
	{
		// TODO Load all levels here and extract levelnames
		// TODO Display in a list
	}

	public void LoadLevel(int id)
	{
		PlayerPrefs.GetString (levelSaveKey + id);
		// TODO Display level to previes here
	}

	string GetLevelPref(int id)
	{
		return PlayerPrefs.GetString (levelSaveKey + id);
	}

	string GetLevelName(int id)
	{
		string name = GetLevelPref (id).Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None)[0];
		print (name);
		return name;
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
			if (GetLevelName (i).Length > 0)
			{
				GameObject obj = Instantiate (levelItemPrefab) as GameObject;
				obj.transform.SetParent (levelViewParent);
				obj.GetComponent<LevelItem>().Initialize (i, GetLevelName (i), this);
				levelItems.Add (obj);
			}
		}
	}

	int GetLevelIdFromPrefs()
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
	/*
	 * 1. Turn GO grid into string
	 * 2. Save string
	 * 3. Load string
	 * 4. Convert string to grid
	 */


	/*void Start()
	{
		input.text = "00000000\n00000000\n00000000\n00000000\n00000000\n00000000";
		prevText = input.text;
	}

	public void GenerateLevel()
	{
		string text = input.text;

		List<string> rows = text.LinesToList ("\n", false);
		Vector3 pos = Vector3.zero;
		for (int y = 0; y < rows.Count; y ++)
		{
			char[] chars = rows[y].ToCharArray ();
			for (int x = 0; x < chars.Length; x ++)
			{
				int id = chars[x] - 48;
				if (bricks[id])
				{
					Instantiate (bricks[id], pos, Quaternion.identity);
				}
				pos.x += brickWidth;
			}
			pos.x = 0f;
			pos.y -= brickHeight;
		}
	}

	public void ValidateInput(string text)
	{
		// TODO check if character is valid, then replace pressed character with 0
		// TODO call on valuechange
		string newText = text;

		if (text.Length > prevText.Length) // Inputted character
		{
			for (int i = 0; i < text.Length; i ++)
			{
				if (i >= prevText.Length || text[i] != prevText[i])
				{

				}
			}
		}
		else // Removed character
		{

		}
		text = text.Replace ("\n", "");
		for (int i = 0; i < text.Length; i ++)
		{
			if (i % 9 == 0)
			{
				text = text.Insert (i, "\n");
				print ("linebreak");
			}
		}
		text = text.Remove (0, 1);


		input.text = text;
		prevText = text;
	}

	bool ValidCharacter(char chr)
	{
		// TODO check if character is inside bricks array range
		return chr - 48 >= 0 && chr - 48 < bricks.Length;
	}*/
}
