using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
	[SerializeField]
	private Transform destroyedPrefab;
	[SerializeField]
	private float destructionForce = 150f;
	[SerializeField]
	private float destructionForceRange = 10f;

	public void Damage()
	{
		Transform destroyedTransform = Instantiate(destroyedPrefab, transform.position, transform.rotation);

		ApplyForceToDestroyed(destroyedTransform, destructionForce, transform.position, destructionForceRange);

		Destroy(gameObject);
	}

	private void ApplyForceToDestroyed(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
	{
		foreach (Transform child in root)
		{
			if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
			{
				childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
			}

			ApplyForceToDestroyed(child, explosionForce, explosionPosition, explosionRange);
		}
	}
}
