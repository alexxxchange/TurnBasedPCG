using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
	[SerializeField]
	private Transform ragdollRootBone;
	[SerializeField]
	private float ragdollForce = 300f;
	[SerializeField]
	private float ragdollForceRange = 10f;


	public void Setup(Transform originalRootBone)
	{
		MatchAllChildTransforms(originalRootBone, ragdollRootBone);

		Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
		ApplyForceToRagdoll(ragdollRootBone, ragdollForce, transform.position + randomDirection, ragdollForceRange);
	}

	private void MatchAllChildTransforms(Transform root, Transform clone)
	{
		foreach(Transform child in root)
		{
			Transform cloneChild = clone.Find(child.name);
			if(cloneChild != null)
			{
				cloneChild.position = child.position;
				cloneChild.rotation = child.rotation;

				MatchAllChildTransforms(child, cloneChild);
			}
		}
	}

	private void ApplyForceToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
	{
		foreach (Transform child in root)
		{
			if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
			{
				childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
			}

			ApplyForceToRagdoll(child, explosionForce, explosionPosition, explosionRange);
		}
	}
}
