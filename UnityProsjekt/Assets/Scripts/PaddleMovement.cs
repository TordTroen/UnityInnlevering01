﻿using UnityEngine;
using System.Collections;

public class PaddleMovement : MonoBehaviour
{
	public float speed = 10f;
	public KeyCode leftKey = KeyCode.LeftArrow;
	public KeyCode rightKey = KeyCode.RightArrow;

	void Update()
	{
		float h = 0f;//Input.GetAxis ("Horizontal");
		if (Input.GetKey (leftKey))
		{
			h = -1f;
		}
		if (Input.GetKey (rightKey))
		{
			h = 1f;
		}
		transform.Translate (new Vector3(h * speed, 0f, 0f) * Time.deltaTime);
	}
}