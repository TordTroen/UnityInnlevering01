using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour
{
	public int scoreReward = 1;

	void OnCollisionEnter2D(Collision2D other)
	{
		gameObject.SetActive (false);
	}
}