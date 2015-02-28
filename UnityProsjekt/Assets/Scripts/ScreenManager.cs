using UnityEngine;
using System.Collections;

public class ScreenManager : MonoBehaviour
{
	public float wallInset = 0.2f; // Wall inset from the egde (0 means no walls are visible, positive = furter in screen)
	public Transform[] walls; // [0] - bottom, [1] - left, [2] - top, [3] - right

	public int pixelsPerUnit = 100; // Pixels per unity (the same as in each sprite's import settings

	void Awake()
	{
		// Set camera size depending on screen height and pixelsperunits (in spritesettings)
		Camera.main.orthographicSize = (Screen.height * 0.5f) / pixelsPerUnit;
		GameManager.instance.UpdateScreenBounds ();

		// Position walls at start of game
		ArrangeWalls ();
	}

	void ArrangeWalls()
	{
		GameManager.instance.UpdateScreenBounds (); // Make sure screenbounds are updated
		Vector3 bl = GameManager.instance.bottomLeft; // Bottom left corner in world coordinates
		Vector3 tr = GameManager.instance.topRight; // Top right corner in world coordinates
		Vector3 origo = GameManager.instance.centerOfScreen; // Center of screen in world coordinates
		float height = Camera.main.orthographicSize * 2.0f; // Height of screen in Unity meters
		float width = height * Screen.width / Screen.height; // Width of screen in Unity meters
		float wallMargin = 0.5f; // Must be half the width of the walls

		// Set the scale of the walls
		walls[0].localScale = new Vector3(width * 2f, 1f); // Bottom
		walls[1].localScale = new Vector3(1f, height * 2f); // Left
		walls[2].localScale = new Vector3(width * 2f, 1f); // Top
		walls[3].localScale = new Vector3(1f, height * 2f); // Right

		// Alter insets depening on how many players (the wall behind a player is put ouside the screen for ball detection)
		float[] insets = new float[]{1f, 1f, 1f, 1f};
		for (int i = 0; i < (int)GameManager.instance.playerMode; i ++)
		{
			insets[i] = 0.5f; // Invert inset to put outisde screen, instead of outside
		}

		// Set the position of the walls
		walls[0].position = new Vector3(origo.x, bl.y - wallMargin + (wallInset * insets[0])); // Bottom
		walls[1].position = new Vector3(bl.x - wallMargin + (wallInset * insets[1]), origo.y); // Left
		walls[2].position = new Vector3(origo.x, tr.y + wallMargin - (wallInset * insets[2])); // Top
		walls[3].position = new Vector3(tr.x + wallMargin - (wallInset * insets[3]), origo.y); // Right

		// Assign the wall IDs
		for (int i = 0; i < walls.Length; i ++)
		{
			walls[i].GetComponent<Wall>().wallId = i;
		}
	}
}