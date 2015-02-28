using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour
{
	public int scoreReward = 1;
	public int curHealth;
	public int maxHealth = 1;
	public float shrinkSpeed = 1f;
	private bool shrinking = false;
	private BoxCollider2D boxCollider;

	void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D>();
	}

	void Start()
	{
		curHealth = maxHealth;
	}

	void Update()
	{
		if (shrinking)
		{
			Vector3 scale = transform.localScale;
			scale -= Vector3.one * shrinkSpeed * Time.deltaTime;
			if (scale.x <= 0.02f)
			{
				gameObject.SetActive (false);
			}
			transform.localScale = scale;
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Ball")
		{
			if (!other.gameObject.GetComponent<BallMovement>().inCooldown)
			{
				curHealth --;
				if (curHealth <= 0)
				{
					//gameObject.SetActive (false);
					boxCollider.enabled = false;
					shrinking = true;
				}
			}
		}
	}
}