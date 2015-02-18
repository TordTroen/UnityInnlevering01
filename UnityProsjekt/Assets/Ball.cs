using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
	public float speed = 10f;

	private Rigidbody2D rbody;

	void Awake()
	{
		rbody = GetComponent<Rigidbody2D>();
	}

	public void PlayBall()
	{
		rbody.isKinematic = false;
		int dir = 1;
		if (Random.Range (0f, 1f) > 0.5f)
		{
			dir = -1;
		}
		rbody.AddForce (new Vector2(speed * dir, speed));
	}
}