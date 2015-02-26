using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
	public int wallId;

	void OnCollisionEnter2D(Collision2D other)
	{
		print (other.gameObject.name + " hit wall " + wallId);
	}
}