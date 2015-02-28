using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
	public int wallId;

	void OnCollisionEnter2D(Collision2D other)
	{
		if (GameManager.instance.playerMode == PlayerMode.Single && wallId == 0)
		{
			Application.LoadLevel (Application.loadedLevel);
		}
		//print (other.gameObject.name + " hit wall " + wallId);
	}
}