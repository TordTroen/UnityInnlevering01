using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	[HideInInspector]public Vector3 topRight; // Top right of screen in world coordinates
	[HideInInspector]public Vector3 bottomLeft; // Bottom left of screen in world coordinates

	void Awake()
	{
		UpdateScreenBounds ();
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