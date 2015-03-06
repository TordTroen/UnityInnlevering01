using UnityEngine;
using System.Collections;

public class AutoDestructParticleSystem : MonoBehaviour 
{
	void Update()
	{
		// When not alive, destroy particle system to prevent countless aprticlesystems to persist after they have played
		if (!particleSystem.IsAlive ())
		{
			Destroy (gameObject);
		}
	}
}
