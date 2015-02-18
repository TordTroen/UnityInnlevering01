using UnityEngine;
using System.Collections;

public class WalManager : MonoBehaviour
{
	public Transform[] walls;

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

		walls[0].position = new Vector3(bl.x - wallMargin, bl.y + height);
		walls[1].position = new Vector3(bl.x + width, tr.y + wallMargin);
		walls[2].position = new Vector3(tr.x + wallMargin, tr.y - height);
		walls[3].position = new Vector3(tr.x - width, bl.y - wallMargin);

		walls[0].localScale = new Vector3(1f, height * 2f);
		walls[1].localScale = new Vector3(width * 2f, 1f);
		walls[2].localScale = new Vector3(1f, height * 2f);
		walls[3].localScale = new Vector3(width * 2f, 1f);
	}
}