using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	[HideInInspector]public Vector3 topRight; // topRight.x = høyre kant, topRight.y = topp kant
	[HideInInspector]public Vector3 bottomLeft; // bottomLeft.x = venstre kant, bottomLeft.y = nedre kant

	public static GameManager instance = null;

	void Awake()
	{
		instance = this;

		UpdateScreenBounds ();
	}

	/// <summary>
	/// Updates the screen bounds.
	/// </summary>
	void UpdateScreenBounds()
	{
		Camera cam = Camera.main;
		bottomLeft = cam.ScreenToWorldPoint (new Vector3 (0f, 0f));
		topRight = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth, cam.pixelHeight));
	}
}