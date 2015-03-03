using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Brick : MonoBehaviour
{
	public int scoreReward = 1;
	private int curHealth;
	public int maxHealth = 1;
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
		if (shrinking)
		{
			Vector3 scale = transform.localScale;
			scale -= Vector3.one * shrinkSpeed * Time.deltaTime;
			if (scale.x <= 0.02f)
			{
				gameObject.SetActive (false);
			}
			transform.localScale = scale;
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Ball")
		{
			if (!other.gameObject.GetComponent<BallMovement>().inCooldown)
			{
				curHealth --;
				if (curHealth <= 0)
				{
					//gameObject.SetActive (false);
					boxCollider.enabled = false;
					shrinking = true;
					curHealth = 0;
				}
				SetColor ();
			}
		}
	}

	void SetColor()
	{
		// Set spritecolor to the correct color from the GameManagers brickColors array
		spriteRenderer.color = GameManager.instance.brickColors[curHealth];
	}
}