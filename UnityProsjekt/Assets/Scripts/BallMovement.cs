using UnityEngine;
using System.Collections;

public class BallMovement : MonoBehaviour {


	public float movementSpeed;
	private float xDir = 1;
	private float yDir = 1;
	private Vector3 oldVector;
	public bool hasStarted;
	public bool inCooldown = false;
	private int hit = 0; // DEBUG Remove this

	void Update(){
		if(Input.GetButtonDown("Jump")){
			PlayBall ();
		}
	}

	void FixedUpdate () {
		if(hasStarted){
			oldVector = new Vector3 (xDir, yDir, 0.0f);
			rigidbody2D.velocity = new Vector3(movementSpeed * xDir, movementSpeed * yDir, 0f) * Time.deltaTime;
		}
	}

	public void PlayBall()
	{
		transform.SetParent (null);
		hasStarted = true;
	}

	public void ChangeDirectionX(Vector3 normal){
		float yNormal = normal.y;
		float xNormal = normal.x;
		xDir = oldVector.x - (2 * ((xNormal * oldVector.x + yNormal * oldVector.y) * xNormal));
		//xDir = Mathf.Round (xDir);
	}

	public void ChangeDirectionY(Vector3 normal){
		float xNormal = normal.x;
		float yNormal = normal.y;
		yDir = oldVector.y - (2 * ((xNormal * oldVector.x + yNormal * oldVector.y) * yNormal));
		//yDir = Mathf.Round (yDir);
	}

	void ChangeDirection(Vector3 normal)
	{
		xDir = oldVector.x - (2 * ((normal.x * oldVector.x + normal.y * oldVector.y) * normal.x));
		yDir = oldVector.y - (2 * ((normal.x * oldVector.x + normal.y * oldVector.y) * normal.y));
	}

	void OnCollisionEnter2D(Collision2D col){
		/*Debug.Log ("1 " + col.gameObject.tag);
		if (col.gameObject.tag == "PlayerBall") {
			if(gameObject.tag == "UpperWall")
			{
				Vector3 normal = col.contacts[0].normal;
				col.gameObject.GetComponent<BallMovementVectoring>().ChangeDirectionY(normal);
				
			}
			
			if(gameObject.tag == "LeftWall"){
				Vector3 normal = col.contacts[0].normal;
				col.gameObject.GetComponent<BallMovementVectoring>().ChangeDirectionX(normal);
			}
			if(gameObject.tag == "RightWall"){
				Vector3 normal = col.contacts[0].normal;
				col.gameObject.GetComponent<BallMovementVectoring>().ChangeDirectionX(normal);
			} 
			
			if(gameObject.tag == "Paddle"){
				Debug.Log("The ball hit the paddle");
				Vector3 normal = col.contacts[0].normal;
				col.gameObject.GetComponent<BallMovementVectoring>().ChangeDirectionY(normal);
			}
		}*/

		if (hasStarted) // Check if we have started (in case the ball hits something before starting to play)
		{
			// Start cooldown (to prevent taking out multiple bricks at the same time)
			if (col.gameObject.tag == "Brick")
			{
				//inCooldown = true; // Set to incooldown = true
				StartCoroutine (StartCooldown ());
				CancelInvoke ("EndCooldown"); // End current invoke, incase one is already running
				Invoke ("EndCooldown", 0.05f); // Invoke endcooldown
			}


			Vector3 norm = col.contacts [0].normal;
			/*if (norm.x <= 1f && norm.x >= -1f) { // Horisontalt
				//print ("hit horizontal");
				ChangeDirectionY (norm);
			}
			if (norm.y <= 1f && norm.y >= -1f) { // Vertikalt
				//print ("hit vertical");
				ChangeDirectionX (norm);
			}
			else
			{
				Debug.Log ("Hit some weird-ass shape [" + col.gameObject + "]");
			}*/
			ChangeDirection (norm);
			//print (hit + "_" + norm + "_" + col.contacts.Length);
			hit ++; // DEBUG Remove this


			// TODO Change xDir when hitting paddle based on distance from center of paddle
		}
	}

	/// <summary>
	/// Ends the cooldown.
	/// </summary>
	void EndCooldown()
	{
		inCooldown = false;
	}

	IEnumerator StartCooldown()
	{
		yield return new WaitForEndOfFrame();
		inCooldown = true;
	}
}
