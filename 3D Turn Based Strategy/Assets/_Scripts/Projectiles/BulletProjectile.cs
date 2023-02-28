using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
	[SerializeField]
	private TrailRenderer trailRenderer;
	[SerializeField]
	private Transform bulletHitVFXPrefab;

	[SerializeField]
	private float moveSpeed = 200f;

	private Vector3 targetPosition;

	public void Setup(Vector3 targetPosition)
	{
		this.targetPosition = targetPosition;
	}

	private void Update()
	{
		Vector3 moveDirection = (targetPosition - transform.position).normalized;

		float sqrMagnitudeBeforeMoving = (transform.position - targetPosition).sqrMagnitude;
		transform.position += moveDirection * moveSpeed * Time.deltaTime;
		float sqrMagnitudeAfterMoving = (transform.position - targetPosition).sqrMagnitude;

		if (sqrMagnitudeBeforeMoving < sqrMagnitudeAfterMoving)
		{
			transform.position = targetPosition;
			trailRenderer.transform.parent = null;
			Destroy(gameObject);
			Instantiate(bulletHitVFXPrefab, targetPosition, Quaternion.identity);
		}

	}

}
