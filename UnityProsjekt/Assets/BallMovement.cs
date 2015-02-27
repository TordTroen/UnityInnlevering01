using UnityEngine;
using System.Collections;

public class BallMovement : MonoBehaviour {


	public float movementSpeed;
	public float xDir;
	public float yDir;
	public Vector3 oldVector;
	public bool hasStarted;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Jump")){
			transform.parent = null;
			hasStarted = true;
		}
		if(hasStarted){
		oldVector = new Vector3 (xDir, yDir, 0.0f);
		rigidbody2D.velocity = new Vector3(movementSpeed * xDir, movementSpeed * yDir, 0f) * Time.deltaTime;
		}
	}

	public void ChangeDirectionX(Vector3 normal){
		Debug.Log ("Try change y direction");
		float yNormal = normal.y;
		float xNormal = normal.x;
		xDir = oldVector.x - (2 * ((xNormal * oldVector.x + yNormal * oldVector.y) * xNormal));
	}

	public void ChangeDirectionY(Vector3 normal){
		float xNormal = normal.x;
		float yNormal = normal.y;
		yDir = oldVector.y - (2 * ((xNormal * oldVector.x + yNormal * oldVector.y) * yNormal));
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
		Vector3 norm = -col.contacts [0].normal;
		if (norm.x < 0.1f && norm.x > -0.1f) { // Horisontalt
			print ("hit horizontal");
			ChangeDirectionY (norm);
		} else if (norm.y < 0.1f && norm.y > -0.1f) { // Horisontalt
			print ("hit vertical");
			ChangeDirectionX (norm);
		}
	}
}
