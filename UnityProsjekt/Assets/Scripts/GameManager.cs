using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public PlayerMode playerMode = PlayerMode.Single; // Playermode (amount of players)

	[HideInInspector]public Vector3 topRight; // topRight.x = right edge, topRight.y = top edge
	[HideInInspector]public Vector3 bottomLeft; // bottomLeft.x = left edge, bottomLeft.y = bottom edge
	public static GameManager instance = null;

	void Awake()
	{
		instance = this;
	}

	/// <summary>
	/// Updates the screen bounds.
	/// </summary>
	public void UpdateScreenBounds()
	{
		Camera cam = Camera.main;
		bottomLeft = cam.ScreenToWorldPoint (new Vector3 (0f, 0f));
		topRight = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth, cam.pixelHeight));
	}
}

public enum PlayerMode
{
	Single = 1,
	Two = 2,
	Three = 3,
	Four = 4
}