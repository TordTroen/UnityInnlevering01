 using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour {

	public float paddleSpeed = 1f;
	private float paddleSizeX;
	private float paddleSizeY;

	void Awake(){
		// For å avgjøre grensen til paddlen
		paddleSizeX = GetComponent<SpriteRenderer>().sprite.bounds.extents.x * 0.5f; // Husk å endre når vi legger til vegger
		paddleSizeY = GetComponent<SpriteRenderer>().sprite.bounds.extents.y * 0.5f;
	}
	// Update is called once per frame
	void Update () {

		float xMovement = ((Input.GetAxis ("Horizontal") * paddleSpeed) * Time.deltaTime) + transform.position.x; // Endring i positionen til paddle'en
		// Erstatter gammel position med ny position
		transform.position = 
		new Vector3 (Mathf.Clamp(xMovement, GameManager.instance.bottomLeft.x + paddleSizeX, 
			                         GameManager.instance.topRight.x - paddleSizeX), 
			            			 GameManager.instance.bottomLeft.y + paddleSizeY, 0.0f);
	}
}
