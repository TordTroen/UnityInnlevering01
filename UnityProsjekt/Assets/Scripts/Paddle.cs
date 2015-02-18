using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour
{
	public float speed = 10f;
	public KeyCode leftKey = KeyCode.LeftArrow;
	public KeyCode rightKey = KeyCode.RightArrow;
	public KeyCode actionKey = KeyCode.Space;

	public bool holdingBall = true;
	private Ball ball;

	void Awake()
	{
		ball = GameObject.FindGameObjectWithTag (Tags.ball).GetComponent<Ball>();
	}

	void Update()
	{
		if (Input.GetKeyDown (actionKey) && holdingBall)
		{
			holdingBall = false;
			ball.PlayBall ();
		}

		float h = 0f;
		if (Input.GetKey (leftKey))
		{
			h = -1f;
		}
		if (Input.GetKey (rightKey))
		{
			h = 1f;
		}
		transform.Translate (new Vector3(h * speed, 0f, 0f) * Time.deltaTime);
	}
}