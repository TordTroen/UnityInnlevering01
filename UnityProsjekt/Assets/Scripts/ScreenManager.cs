using UnityEngine;
using System.Collections;

public class ScreenManager : MonoBehaviour
{
	public float inset = 0.2f;
	public Transform[] walls; // [0] - left, [1] - top, [2] - right, [3] - bottom

	public int pixelsPerUnit = 100;
	public Camera cam;
	
	void Awake()
	{
		if (!cam)
		{
			cam = Camera.main;
		}
		cam.orthographicSize = (Screen.height * 0.5f) / pixelsPerUnit;
	}

	void Start()
	{
		ArrangeWalls ();
	}

	[ContextMenu("Place walls")]
	void ArrangeWalls()
	{
		Camera cam = Camera.main;
		Vector3 bl = cam.ScreenToWorldPoint (new Vector3(0f, 0f));
		Vector3 tr = cam.ScreenToWorldPoint (new Vector3(cam.pixelWidth, cam.pixelHeight));
		float width = Vector3.Distance (bl, new Vector3(bl.x, tr.y)) * 0.5f;
		float height = Vector3.Distance (tr, new Vector3(tr.x, bl.y)) * 0.5f;
		float wallMargin = 0.5f;

		walls[0].position = new Vector3(bl.x - wallMargin + inset, bl.y + height); // Left
		walls[1].position = new Vector3(bl.x + width, tr.y + wallMargin - inset); // Top
		walls[2].position = new Vector3(tr.x + wallMargin - inset, tr.y - height); // Right
		walls[3].position = new Vector3(tr.x - width, bl.y - wallMargin + inset); // Bottom

		walls[0].localScale = new Vector3(1f, height * 2f); // Left
		walls[1].localScale = new Vector3(width * 2f, 1f); // Top
		walls[2].localScale = new Vector3(1f, height * 2f); // Right
		walls[3].localScale = new Vector3(width * 2f, 1f); // Bottom


		/*
normal ved horisontalt: normal.x = 0
normal ved vertikal: normal.y = 0
		 */
	}
}