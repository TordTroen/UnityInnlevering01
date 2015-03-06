using UnityEngine;
using System.Collections;

public class AutoDestructParticleSystem : MonoBehaviour 
{
	void Update()
	{
		if (!particleSystem.IsAlive ())
		{
			gameObject.SetActive (false);
		}
	}
}
