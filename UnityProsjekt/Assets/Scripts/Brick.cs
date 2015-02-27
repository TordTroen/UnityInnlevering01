using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour
{
	public int scoreReward = 1;
	public int curHealth;
	public int maxHealth = 1;

	void OnEnable()
	{
		curHealth = maxHealth;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		curHealth --;
		if (curHealth <= 0)
		{

			gameObject.SetActive (false);
		}
	}
}