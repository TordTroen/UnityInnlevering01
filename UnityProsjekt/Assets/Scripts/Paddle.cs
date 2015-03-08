 using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour {

	public float paddleSpeed = 1f;
	public KeyCode leftKey = KeyCode.LeftArrow; // We use KeyCodes instead of Buttons from the Input manager so its easier to set on each player
	public KeyCode rightKey = KeyCode.RightArrow;

	public int curHealth; // Player health
	public int maxHealth = 3; // Player max health
	public int score; // Current score

	public GameObject ballPrefab; // Ballprefab for instantiating a ball
	public Sprite[] paddleSprites; // Sprites for full and half size (0 - full, 1 - half)
	public Color paddleColor = Color.blue; // The color of the paddle

	[HideInInspector]public BallMovement ball; // The paddles ball
	[HideInInspector]public int playerId; // Playerid
	private Vector2 paddleSize; // The size of the paddle
	private bool isHalfSize = true; // Whether the paddle is halfsize or not
	private int invert = 1; // -1 = invert the keys
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D paddleCollider;
	private float wallInset; // Inset of the walls

	void Awake(){
		// Get wallinset from screenmanager
		wallInset = GameManager.instance.GetComponent<ScreenManager>().wallInset * 0.5f; // (screenmanager must be on same object as GameManager)
		spriteRenderer = GetComponent<SpriteRenderer>();
		paddleCollider = GetComponent<BoxCollider2D>();
		UpdatePaddleSize ();

		// Instantiate the ball and get it's BallMovement script
		ball = (Instantiate (ballPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<BallMovement>();
		ball.ownerPaddle = this;
		ball.UpdateColor ();
	}

	// Update is called once per frame
	void Update () {
		// Get input from left & right key
		float inputMovement = 0f;
		if (Input.GetKey (rightKey) && GameManager.instance.gameInProgress)
		{
			inputMovement = 1f;
		}
		if (Input.GetKey (leftKey) && GameManager.instance.gameInProgress)
		{
			inputMovement = -1f;
		}

		// Get new position with input applied
		Vector3 pos = transform.position + (((transform.right * inputMovement) * paddleSpeed) * Time.deltaTime) * invert; 

		// Apply new position (Clamped inside the screen)
		transform.position = new Vector3(
			Mathf.Clamp (pos.x, GameManager.instance.bottomLeft.x + paddleSize.x + wallInset,
		             GameManager.instance.topRight.x - paddleSize.x - wallInset),
			Mathf.Clamp (pos.y, GameManager.instance.bottomLeft.y + paddleSize.y + wallInset,
		             GameManager.instance.topRight.y - paddleSize.y - wallInset));
	}

	/// <summary>
	/// Initializes the paddle. Called when instantiating the paddle.
	/// </summary>
	/// <param name="playerId">Player id.</param>
	/// <param name="keyCombo">Key combination.</param>
	public void InitializePaddle(int playerId, KeyCombination keyCombo, Color color)
	{
		// Set color
		paddleColor = color;
		spriteRenderer.color = paddleColor;

		// Set health
		curHealth = maxHealth;

		// Set playerid
		this.playerId = playerId;
		
		// Set keys
		leftKey = keyCombo.leftKey;
		rightKey = keyCombo.rightKey;
		
		// Rotate based on which player this is
		int rotInvert = 1;
		if (playerId == 1 || playerId == 3) rotInvert *= -1; // Invert so players always face towards center
		transform.eulerAngles = new Vector3(0f, 0f, 90f * rotInvert) * playerId; // Rotate player

		// Invert keys so that for player 3 & 4 the keys are correct (rotating affects which keys
		if (playerId == 2 || playerId == 3)
		{
			invert = -1;
		}

		// Reset paddlesize
		SetPaddleSize (true);

		// Initialize the ball
		ResetBall ();

		// Reset score
		IncreaseScore (-score); // Increase score with negative the current score to reset to 0

		// Set paddle position
		Vector3 pos = transform.position;
		transform.position = pos;
	}

	public void LoseLife()
	{
		curHealth --;
		PlayerManager.instance.OnPlayerLoseHealth (playerId, !IsAlive ());
		if (IsAlive ()) // If still alive
		{
			SetPaddleSize (true);
		}
		else // If dead
		{
			DeactivatePaddle ();
		}
	}

	/// <summary>
	/// Check if paddle has lives.
	/// </summary>
	/// <returns><c>true</c> if health > 0; otherwise, <c>false</c>.</returns>
	public bool IsAlive()
	{
		return curHealth > 0;
	}

	/// <summary>
	/// Resets the ball.
	/// </summary>
	public void ResetBall()
	{
		ball.UpdateColor ();
		ball.hasStarted = false;
		ball.transform.SetParent (transform);
		ball.transform.position = transform.position + transform.up * 1f;
	}

	/// <summary>
	/// Sets the size of the paddle.
	/// </summary>
	/// <param name="fullSize">If set to <c>true</c> the paddle is set to the normal size. Else the size is halved.</param>
	public void SetPaddleSize(bool fullSize)
	{
		if (fullSize && isHalfSize) // Set to fullsize
		{
			spriteRenderer.sprite = paddleSprites[0]; // Set sprite
			paddleCollider.size = new Vector2(2f, 0.25f); // Set collider size
		}
		else if (!fullSize && !isHalfSize) // Set to halfsize
		{
			spriteRenderer.sprite = paddleSprites[1]; // Set sprite
			paddleCollider.size = new Vector2(1.25f, 0.25f); // Set collider size
		}
		// Update paddlesize
		UpdatePaddleSize ();

		// Update isHalfSize bool
		isHalfSize = !fullSize;
	}

	/// <summary>
	/// Updates the size of the paddle. Call when changing the sprite
	/// </summary>
	void UpdatePaddleSize()
	{
		// For å avgjøre grensen til paddlen
		paddleSize = spriteRenderer.bounds.extents;
	}

	public void IncreaseScore(int toAdd)
	{
		score += toAdd; // Add to score
		GUIManager.instance.UpdatePlayerStats (playerId); // Update score texts
	}

	public void DeactivatePaddle()
	{
		//StartCoroutine (WaitAndDeactivatePaddle ());
		if (ball)
		{
			ball.gameObject.SetActive (false);
		}
		gameObject.SetActive (false);
	}

	public IEnumerator TerminatePaddle()
	{
		yield return new WaitForEndOfFrame(); // Wait to end of frame so other calls to this class does their thing first
		
		// Remove from playermanager and destroy ball + paddle
		PlayerManager.instance.allPaddles.Remove (this);
		if (ball)
		{
			Destroy (ball.gameObject);
		}
		Destroy (gameObject);
	}
}
