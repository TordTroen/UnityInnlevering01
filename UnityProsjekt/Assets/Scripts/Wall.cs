using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
	public int wallId;

	void OnCollisionEnter2D(Collision2D other)
	{
		if (PlayerManager.instance.allPaddles.Count > wallId)
		{
			PlayerManager.instance.allPaddles[wallId].LoseLife ();
		}
		//print (other.gameObject.name + " hit wall " + wallId);
	}
}