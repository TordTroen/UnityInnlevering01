 using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour {

	public float paddleSpeed = 1f;
	public KeyCode leftKey = KeyCode.LeftArrow;
	public KeyCode rightKey = KeyCode.RightArrow;

	public int playerId;
	private float paddleSizeX;
	private float paddleSizeY;
	private Vector2 paddleSize;
	private int invert = 1;

	void Awake(){
		// For å avgjøre grensen til paddlen
		paddleSize = GetComponent<SpriteRenderer>().bounds.extents;
		//paddleSizeX = GetComponent<SpriteRenderer>().sprite.bounds.extents.x; // Husk å endre når vi legger til vegger
		//paddleSizeY = GetComponent<SpriteRenderer>().sprite.bounds.extents.y;
	}

	// Update is called once per frame
	void Update () {
		// Get input from left & right key
		float inputMovement = 0f;
		if (Input.GetKey (rightKey))
		{
			inputMovement = 1f;
		}
		if (Input.GetKey (leftKey))
		{
			inputMovement = -1f;
		}

		// Get new position with input applied
		Vector3 pos = transform.position + (((transform.right * inputMovement) * paddleSpeed) * Time.deltaTime) * invert; 

		// Apply new position (Clamped inside the screen)
		transform.position = new Vector3(
			Mathf.Clamp (pos.x, GameManager.instance.bottomLeft.x + paddleSize.x, GameManager.instance.topRight.x - paddleSize.x),
			Mathf.Clamp (pos.y, GameManager.instance.bottomLeft.y + paddleSize.x, GameManager.instance.topRight.y - paddleSize.x));
		/*float xMovement = ((inputMovement * paddleSpeed) * Time.deltaTime) + transform.position.x; // Endring i positionen til paddle'en
		// Erstatter gammel position med ny position
		transform.position = new Vector3 (Mathf.Clamp(xMovement, GameManager.instance.bottomLeft.x + paddleSizeX, 
			                         GameManager.instance.topRight.x - paddleSizeX), 
			            			 GameManager.instance.bottomLeft.y + paddleSizeY + 0.75f, 0.0f);*/
	}

	/// <summary>
	/// Initializes the paddle. Called when instantiating the paddle.
	/// </summary>
	/// <param name="playerId">Player id.</param>
	/// <param name="keyCombo">Key combination.</param>
	public void InitializePaddle(int playerId, KeyCombination keyCombo)
	{
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
		if (playerId == 1 || playerId == 2)
		{
			invert = -1;
		}
	}
}
