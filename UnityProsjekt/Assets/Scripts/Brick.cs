using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Brick : MonoBehaviour
{
	public int scoreReward = 1;
	private int curHealth;
	public int maxHealth = 1;
	public bool independantColor = false;
	public GameObject particles;
	private float shrinkSpeed = 10f;
	private bool shrinking = false;
	private BoxCollider2D boxCollider;
	private SpriteRenderer spriteRenderer;
	
	void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Start()
	{
		curHealth = maxHealth;
		SetColor ();
	}
	
	void Update()
	{
		// Brick shrinking
		if (shrinking)
		{
			Vector3 scale = transform.localScale; // Get current scale
			scale -= Vector3.one * shrinkSpeed * Time.deltaTime; // Decrease scale
			if (scale.x <= 0.02f) // If scale is small enough
			{
				Destroy (gameObject); // Remove brick
			}
			transform.localScale = scale; // Apply new scale
		}
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Ball")
		{
			// Get reference to ball that hit this brick
			BallMovement hitBall = other.collider.GetComponent<BallMovement>();
			
			// Don't remove if ball has hit more than one thing at the same time
			if (hitBall.currentHits > 1)
			{
				return;
			}
			
			// If orange or red brick, increase ballspeed
			if (maxHealth == 2 || maxHealth == 3)
			{
				hitBall.IncreaseSpeed ();
			}
			
			// Increase score
			hitBall.ownerPaddle.IncreaseScore (scoreReward);
			
			// Decrease health
			curHealth --;
			
			// Check if brick has more health
			if (curHealth <= 0)
			{
				// Disable collider
				boxCollider.enabled = false;
				// Start shrinking
				shrinking = true;
				// Clamp health so it doesn't go below 0 (so it is within the range of the colors array)
				curHealth = 0;

				// Instantiate brick particlesystem
				// TODO Implement pooling for particles
				particles.GetComponent<ParticleSystem>().startColor = spriteRenderer.color;
				Instantiate(particles, transform.position, Quaternion.identity);
			}
			
			// Update color
			SetColor ();
		}
	}
	
	void SetColor()
	{
		if (!independantColor)
		{
			// Set spritecolor to the correct color from the GameManagers brickColors array
			spriteRenderer.color = GameManager.instance.brickColors[curHealth];
		}
	}
}