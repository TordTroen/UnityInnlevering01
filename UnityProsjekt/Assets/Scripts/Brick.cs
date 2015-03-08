using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Brick : MonoBehaviour
{
	public int scoreReward = 1; // Amount of points that are awarded on destroy
	private int curHealth; // Current brick health
	public int maxHealth = 1; // Max brick health
	public bool indestructible = false; // If true, the brick can't be destroyed
	public bool independantColor = false; // Independant color (from changing color when being hit)
	public GameObject hitParticle; // Particlesystem when being hit
	[HideInInspector]public LevelInfo levelInfoParent; // LevelInfo on the parent object
	private float shrinkSpeed = 10f; // Shrink speed when destroyed
	private bool shrinking = false; // Shrinking when true
	private BoxCollider2D boxCollider;
	private SpriteRenderer spriteRenderer;
	
	void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Start()
	{
		// Set health and color
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

	/// <summary>
	/// Called when hit by a ball.
	/// </summary>
	/// <param name="ball">The ball that hit.</param>
	public void OnHit(BallMovement ball)
	{
		// Play same sound as when hitting a wall and stop the function if the brick is indestructible
		if (indestructible)
		{
			GameManager.instance.PlayAudioClip (
				GameManager.instance.acWallHits[Random.Range (0, GameManager.instance.acWallHits.Length)]);
			return;
		}

		// Play brick hit audio
		GameManager.instance.PlayAudioClip (GameManager.instance.acBrickHit);

		// If orange or red brick, increase ballspeed
		if (maxHealth == 2 || maxHealth == 3 || gameObject.name.Contains ("red") || gameObject.name.Contains ("orange"))
		{
			ball.IncreaseSpeed ();
		}
		
		// Decrease health
		curHealth --;
		
		// Check if brick has more health
		if (curHealth <= 0)
		{
			// Increase score
			ball.ownerPaddle.IncreaseScore (scoreReward);
			
			// Disable collider
			boxCollider.enabled = false;
			// Start shrinking
			shrinking = true;
			// Clamp health so it doesn't go below 0 (so it is within the range of the colors array)
			curHealth = 0;
			
			// Instantiate brick particlesystem
			// TODO Implement pooling for particles
			hitParticle.GetComponent<ParticleSystem>().startColor = spriteRenderer.color;
			Instantiate(hitParticle, transform.position, Quaternion.identity);

			// Remove brick from parent levelinfo
			levelInfoParent.OnBrickDestroyed (gameObject);
		}
		
		// Update color
		SetColor ();
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