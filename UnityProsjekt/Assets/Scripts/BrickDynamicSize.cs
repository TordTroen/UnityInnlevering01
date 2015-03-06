using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// this script is used for the classic breakout level
public class BrickDynamicSize : MonoBehaviour {
	public GameObject[] brickLayers;
	public int bricksInRow = 8;
	public float tileHeight = 0.25f;
	public float offsetFromCenter;
	public Vector2 colliderMod;

	void Start () {
		Camera cam = Camera.main;
		List<Transform> bricks = new List<Transform> ();
		for(int row = 0; row < brickLayers.Length; row++){
			for (int j = 0; j < bricksInRow; j ++)
			{
				GameObject obj = Instantiate (brickLayers[row], Vector3.zero, Quaternion.identity) as GameObject;
				obj.transform.SetParent (transform);
				obj.GetComponent<Brick>().independantColor = true;
				bricks.Add(obj.transform);
				obj.GetComponent<Brick>().scoreReward = (row + 1);
				obj.GetComponent<Brick>().maxHealth = 1;
				// colliderMod = 0.75f * obj.GetComponent<BoxCollider2D>().size;
				// obj.GetComponent<BoxCollider2D>().size = colliderMod; // for å unngå flere samtidige collisioner
			}
		
			for(int i = 0; i < bricks.Count; i++){
				Vector3 scale =  new Vector2(
					(Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0.0f)), cam.ScreenToWorldPoint(new Vector3(0f, 0f))) - GameManager.instance.GetComponent<ScreenManager>().wallInset)
					/ (bricks.Count), 1f);

				Vector3 posRight = GameManager.instance.topRight;
				Vector3 updatedPos = new Vector3(posRight.x - (scale.x * i + 0.96f + GameManager.instance.GetComponent<ScreenManager>().wallInset), tileHeight * (row + 1) + offsetFromCenter);
				bricks[i].localScale = scale;
				bricks[i].position = updatedPos;
			}
			bricks = new List<Transform>();
		}
	}
}
