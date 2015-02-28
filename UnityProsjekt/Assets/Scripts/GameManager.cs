using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public PlayerMode playerMode = PlayerMode.Single; // Playermode (amount of players)
	public GameObject paddlePrefab; // Paddle prefab
	public KeyCombination[] keyCombinations; // All player key combinations
	public Transform[] spawnPoints; // Spawnpoints TODO Get these dynamically from each level

	[HideInInspector]public Vector3 topRight; // topRight.x = right edge, topRight.y = top edge
	[HideInInspector]public Vector3 bottomLeft; // bottomLeft.x = left edge, bottomLeft.y = bottom edge
	[HideInInspector]public Vector3 centerOfScreen; // Center of screen in world coordinates (in case playingfield isn't centered in the scene)
	public static GameManager instance = null;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		InitializePlayers (); // TODO put at start of round
	}

	void InitializePlayers()
	{
		for (int i = 0; i < (int)playerMode; i ++)
		{
			GameObject paddleObject = Instantiate (paddlePrefab, spawnPoints[i].position, Quaternion.identity) as GameObject;
			Paddle paddle = paddleObject.GetComponent<Paddle>();
			paddle.InitializePaddle (i, keyCombinations[i]);
		}
	}

	/// <summary>
	/// Updates the screen bounds.
	/// </summary>
	public void UpdateScreenBounds()
	{
		Camera cam = Camera.main;
		bottomLeft = cam.ScreenToWorldPoint (new Vector3 (0f, 0f)); // Bottomleft corner from screen to world coordinates
		topRight = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth, cam.pixelHeight)); // Topright corner from screen to world coordinates
		centerOfScreen = cam.ScreenToWorldPoint (new Vector3(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f)); // Center of screen to world coordinates
		bottomLeft.z = 0f; // Zero out the z
		topRight.z = 0f; // Zero out the z
		centerOfScreen.z = 0f; // Zero out the z
	}
}

// Class of left/right key for player initialization
// 
[System.Serializable]
public class KeyCombination
{
	public KeyCode leftKey = KeyCode.LeftArrow;
	public KeyCode rightKey = KeyCode.RightArrow;
}

public enum PlayerMode
{
	Single = 1,
	Two = 2,
	Three = 3,
	Four = 4
}