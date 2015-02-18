using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
	public float speed = 10f;

	void Update()
	{
		transform.Translate (transform.right * speed * Time.deltaTime);
	}
}