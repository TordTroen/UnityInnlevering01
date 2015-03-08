using UnityEngine;
using System.Collections;

public class BallMovement : MonoBehaviour {

	public float movementSpeed; // The speed of the ball
	public float angleStrength; // The sesitivity of the new angle the ball gets when it collides with the paddle
	public float speedFactor = 1.2f; // Factor for increasing the speed [currentSpeed += (origingalSpeed * speedFactor / 100)]
	public bool hasStarted; // For keeping track of when the ball is supposed to be moving

	private Vector2 oldVector;
	private int bounces = 0; // Keeping track of number of bounces (for increasing speed) 
	private Vector2 direction; // Ball direction
	private float origSpeed; // Start speed of the ball
	private bool canBounce = true; // Used to make sure the ball only hits one brick at the time
	[HideInInspector]public Paddle ownerPaddle; // Which paddle "owns" this ball
	private SpriteRenderer spriteRenderer; // The balls spriterenderer
	
	void Awake()
	{
		// Get the spriterenderer
		spriteRenderer = GetComponent<SpriteRenderer>();
		// Assign originalspeed
		origSpeed = movementSpeed;
	}
	
	void Start(){
		// Set startdirection
		direction = new Vector2 (1f, 1f).normalized;
	}
	
	void Update () {
		if(hasStarted && GameManager.instance.gameInProgress){
			// Move ball
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

	void ChangeDirection(Vector3 normal)
	{
		/* Vi prøvde å implementere formelen, men vi møtte på noen bugs vi ikke fikk løst. Derfor brukte vi bare Unitys Vector3.Reflect.
		 * En Enklere måte hadde bare vært å invertere x og y retningen avhengig av hva ballen traff. Men siden vi vurderte å bruke
		 * brikker som ikke er helt horisontale eller vertikale, brukte vi funksjonen.
		direction = new Vector2(oldVector.x - (2 * ((normal.x * oldVector.x + normal.y * oldVector.y) * normal.x)),
		                        oldVector.y - (2 * ((normal.x * oldVector.x + normal.y * oldVector.y) * normal.y)));*/
		direction = Vector3.Reflect (oldVector, normal);
	}
	
	void OnCollisionEnter2D(Collision2D col){
		if (hasStarted) // Check if we have started (in case the ball hits something before starting to play)
		{
			if (canBounce)
			{
				if(col.gameObject.tag == "Paddle") // Hit paddle
				{
					PaddleCollision(col);
				} 
				else // Hit something else
				{
					// Get the normal and pass it to ChangeDirection()
					Vector3 norm = col.contacts [0].normal;
					ChangeDirection (norm);
				}

				// Toggle canbounce
				canBounce = false;
				if (gameObject.activeInHierarchy) // Check if active (problems if starting coroutines on deactivated objects)
				{
					StartCoroutine (WaitToActivateBounce ());
				}

				// Hit a brick
				if (col.gameObject.CompareTag ("Brick"))
				{
					col.gameObject.GetComponent<Brick>().OnHit (this);
				}
			}

			// Set oldvector to current direction
			oldVector = direction;
			
			// Increase bouncecount
			bounces ++;

			// Increase speed if 4th or 12th bounce
			if (bounces == 4 || bounces == 12)
			{
				IncreaseSpeed ();
			}

			// Play audio if hitting a wall or the paddle
			if (col.gameObject.CompareTag ("Wall") || col.gameObject.CompareTag ("Paddle"))
			{
				GameManager.instance.PlayAudioClip (
					GameManager.instance.acWallHits[Random.Range (0, GameManager.instance.acWallHits.Length)]);
			}
		}
	}

	IEnumerator WaitToActivateBounce()
	{
		// Wait for end of frame to make sure all other collision has happened
		yield return new WaitForEndOfFrame();
		canBounce = true;
	}

	void PaddleCollision(Collision2D col){
		Vector2 paddlePos = col.transform.position;
		Vector2 ballPos = gameObject.transform.position;

		Vector2 difference = ballPos - paddlePos; // this might cause the ball to move too horizontal
		difference.x = difference.x * angleStrength / 100f;
		Vector2 newDirection = difference.normalized;
		direction.y = newDirection.y;
		direction.x = newDirection.x;
	}
	
	/// <summary>
	/// Increases the ball speed.
	/// </summary>
	public void IncreaseSpeed()
	{
		movementSpeed += ((origSpeed * speedFactor) * 0.01f);
	}
	
	/// <summary>
	/// Resets the speed.
	/// </summary>
	void ResetSpeed()
	{
		movementSpeed = origSpeed;
	}

	/// <summary>
	/// Updates the ball color to the same color as the paddle.
	/// </summary>
	public void UpdateColor()
	{
		spriteRenderer.color = ownerPaddle.paddleColor;
	}
}