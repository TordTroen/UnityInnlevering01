using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
	public int wallId;

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag ("Ball"))
		{
			// Check if ball hit wall (if wall is behind a player)
			if (PlayerManager.instance.allPaddles.Count > wallId && PlayerManager.instance.allPaddles[wallId].ball.hasStarted
			    && PlayerManager.instance.allPaddles[wallId].IsAlive ())
			{
				// Lose a life
				PlayerManager.instance.allPaddles[wallId].LoseLife ();

				// Reset the ball that hit the wall
				BallMovement ball = other.gameObject.GetComponent<BallMovement>();
				ball.ownerPaddle.ResetBall ();
				ball.PlayBall ();
			}

			// If topwall and singleplayer
			if (wallId == 2 && GameManager.instance.playerMode == PlayerMode.Single)
			{
				// Set paddle size
				PlayerManager.instance.allPaddles[0].SetPaddleSize (false);
			}
		}
	}
}