using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour
{
	public int pointReward = 1;

	private ScoreManager scoreManager;

	void Awake()
	{
		scoreManager = GameObject.FindGameObjectWithTag (Tags.manager).GetComponent<ScoreManager>();
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		scoreManager.currentScore += pointReward;
		gameObject.SetActive (false);
	}
}