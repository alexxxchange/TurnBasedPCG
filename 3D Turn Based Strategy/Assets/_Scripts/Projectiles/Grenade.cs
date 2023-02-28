using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
	public static  event EventHandler OnAnyGrenadeEvent;

	[SerializeField]
	private Transform grenadeExplosionVFXPrefab;
	[SerializeField]
	private TrailRenderer trailRenderer;
	[SerializeField]
	private AnimationCurve arcYAnimationCurve;
	[SerializeField]
	private float moveSpeed = 15f;
	[SerializeField]
	private float reachedTargetDistance = .2f;
	[SerializeField]
	private float damageRadius = 3f;
	[SerializeField]
	private int damageAmount = 30;

	private Vector3 targetPosition;
	private Action OnGrenadeBehaviourComplete;
	private float totalDistance;
	private Vector3 positionXZ;


	private void Update()
	{
		Vector3 moveDir = (targetPosition - positionXZ).normalized;
		positionXZ += moveDir * moveSpeed * Time.deltaTime;

		float distance = Vector3.Distance(positionXZ, targetPosition);
		float distanceNormalized = 1 - distance / totalDistance;

		float maxHeight = totalDistance / 4f;
		float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
		transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);



		if (Vector3.Distance(transform.position, targetPosition) < reachedTargetDistance)
		{
			Collider[] colliderArray = Physics.OverlapSphere(targetPosition,damageRadius);
			foreach(Collider collider in colliderArray)
			{
				if (collider.TryGetComponent<Unit>(out Unit targetUnit))
				{
					targetUnit.Damage(damageAmount);
				}

				if (collider.TryGetComponent<Destructible>(out Destructible destructible))
				{
					destructible.Damage();
				}
			}

			OnAnyGrenadeEvent?.Invoke(this, EventArgs.Empty);
			trailRenderer.transform.parent = null;
			Instantiate(grenadeExplosionVFXPrefab, transform.position, Quaternion.identity);
			Destroy(gameObject);
			OnGrenadeBehaviourComplete();
		}
	}

	public void Setup(GridPosition targetGridPosition, Action OnGrenadeBehaviourComplete)
	{
		this.OnGrenadeBehaviourComplete = OnGrenadeBehaviourComplete;
		targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
		
		positionXZ = transform.position;
		positionXZ.y = 1;
		totalDistance = Vector3.Distance(positionXZ, targetPosition);
	}
}
