using UnityEngine;
using System.Collections;

public class BallMovement : MonoBehaviour {


	public float movementSpeed;
	public float speedFactor = 1.2f; // Factor for increasing the speed [currentSpeed += (origingalSpeed * speedFactor / 100)]
	public bool hasStarted;
	
	//private float xDir = 1;
	//private float yDir = 1;
	private Vector2 oldVector;
	private int bounces = 0;
	private Vector2 direction;
	private float origSpeed;
	[HideInInspector]public Paddle ownerPaddle;
	public int currentHits;
	private SpriteRenderer spriteRenderer;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		origSpeed = movementSpeed;
	}

	void Start(){
		direction = new Vector2 (1f, 1f).normalized;
	}

	void FixedUpdate () {
		if(hasStarted){
			rigidbody2D.velocity = direction * movementSpeed * Time.deltaTime;
		}
	}

	/// <summary>
	/// Starts the ball.
	/// </summary>
	public void PlayBall()
	{
		// Reset speed
		ResetSpeed ();

		// Set oldvector
		oldVector = new Vector2 (direction.x, direction.y);

		// Unparent from paddle
		transform.SetParent (null);

		// Set to hasstarted
		hasStarted = true;

		// Reset bounces
		bounces = 0;
	}

	public void ChangeDirectionX(Vector3 normal){
		float yNormal = normal.y;
		float xNormal = normal.x;
		direction.x = oldVector.x - (2 * ((xNormal * oldVector.x + yNormal * oldVector.y) * xNormal));
	}

	public void ChangeDirectionY(Vector3 normal){
		float xNormal = normal.x;
		float yNormal = normal.y;
		direction.y = oldVector.y - (2 * ((xNormal * oldVector.x + yNormal * oldVector.y) * yNormal));
	}

	void ChangeDirection(Vector3 normal)
	{
		//direction.x = oldVector.x - (2 * ((normal.x * oldVector.x + normal.y * oldVector.y) * normal.x));
		//direction.y = oldVector.y - (2 * ((normal.x * oldVector.x + normal.y * oldVector.y) * normal.y));
		direction = Vector3.Reflect (oldVector, normal);
		return;
		if (normal.x <= 1f && normal.x >= -1f) { // Horisontalt
			//print ("hit horizontal");
			direction.y *= -1;
		}
		else if (normal.y <= 1f && normal.y >= -1f) { // Vertikalt
			//print ("hit vertical");
			direction.x *= -1;
		}
		else
		{
			Debug.Log ("Hit some weird-ass shape");
		}
	}

	void OnCollisionEnter2D(Collision2D col){
		if (hasStarted) // Check if we have started (in case the ball hits something before starting to play)
		{
			if(col.gameObject.tag == "Paddle") 
			{
				SetOwnerPaddle (col.gameObject.GetComponent<Paddle>());
				PaddleCollision(col);
			} 
			else
			{
				Vector3 norm = col.contacts [0].normal;
				//ChangeDirection (norm);
				ChangeDirection (norm);
			}

			oldVector = new Vector2 (direction.x, direction.y);

			// Increase bouncecount
			bounces ++;
			// Increase speed if 4th or 12th bounce
			if (bounces == 4 || bounces == 12)
			{
				IncreaseSpeed ();
			}
			currentHits ++;
		}
	}

	void OnCollisionExit2D(Collision2D col)
	{
		currentHits = 0;
	}

	void PaddleCollision(Collision2D col){
		Vector2 paddlePos = col.transform.position;
		Vector2 ballPos = gameObject.transform.position;
		Vector2 difference = ballPos - paddlePos;
		Vector2 newDirection = difference.normalized;
		direction.y = newDirection.y;
		direction.x = newDirection.x;
	}

	/// <summary>
	/// Increases the ball speed.
	/// </summary>
	public void IncreaseSpeed()
	{
		movementSpeed += (origSpeed * speedFactor) * 0.01f;
	}

	/// <summary>
	/// Resets the speed.
	/// </summary>
	void ResetSpeed()
	{
		movementSpeed = origSpeed;
	}

	public void UpdateColor()
	{
		spriteRenderer.color = ownerPaddle.paddleColor;
	}

	public void SetOwnerPaddle(Paddle newOwner)
	{
		//ownerPaddle = newOwner;
		UpdateColor ();
	}
}