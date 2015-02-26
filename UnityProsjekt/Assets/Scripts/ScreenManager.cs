using UnityEngine;
using System.Collections;

public class ScreenManager : MonoBehaviour
{
	public float wallInset = 0.2f; // Wall inset from the egde (0 means no walls are visible, positive = furter in screen)
	public Transform[] walls; // [0] - left, [1] - top, [2] - right, [3] - bottom

	public int pixelsPerUnit = 100; // Pixels per unity (the same as in each sprite's import settings

	void Awake()
	{
		// Set camera size depending on screen height and pixelsperunits (in spritesettings)
		Camera.main.orthographicSize = (Screen.height * 0.5f) / pixelsPerUnit;
		GameManager.instance.UpdateScreenBounds ();
	}

	void Start()
	{
		// Position walls at start of game
		ArrangeWalls ();
	}

	void ArrangeWalls()
	{
		Vector3 bl = GameManager.instance.bottomLeft; // Bottom left corner in world coordinates
		Vector3 tr = GameManager.instance.topRight; // Top right corner in world coordinates
		float width = Vector3.Distance (bl, new Vector3(bl.x, tr.y)) * 0.5f; // Screen width in Unity meters
		float height = Vector3.Distance (tr, new Vector3(tr.x, bl.y)) * 0.5f; // Screen height in Unity meters
		float wallMargin = 0.5f; // Must be half the width of the walls

		// Set the scale of the walls
		walls[0].localScale = new Vector3(1f, height * 2f); // Left
		walls[1].localScale = new Vector3(width * 2f, 1f); // Top
		walls[2].localScale = new Vector3(1f, height * 2f); // Right
		walls[3].localScale = new Vector3(width * 2f, 1f); // Bottom

		// Set the position of the walls
		walls[0].position = new Vector3(bl.x - wallMargin + wallInset, bl.y + height * 0.25f); // Left
		walls[1].position = new Vector3(bl.x + width * 0.75f, tr.y + wallMargin - wallInset); // Top
		walls[2].position = new Vector3(tr.x + wallMargin - wallInset, tr.y - height * 0.25f); // Right
		walls[3].position = new Vector3(tr.x - width * 0.75f, bl.y - wallMargin - wallInset); // Bottom
	}
}