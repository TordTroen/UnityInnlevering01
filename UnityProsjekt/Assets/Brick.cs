﻿using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D other)
	{
		gameObject.SetActive (false);
	}
}