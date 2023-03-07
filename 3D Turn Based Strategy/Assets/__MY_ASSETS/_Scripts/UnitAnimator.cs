using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
	[SerializeField]
	private Animator unitAnimator;
	[SerializeField]
	private Transform bulletProjectilePrefab;
	[SerializeField]
	private Transform shootPointTransform;

	private void Awake()
	{
		if(TryGetComponent<MoveAction>(out MoveAction moveAction))
		{
			moveAction.OnStartMoving += MoveAction_OnStartMoving;
			moveAction.OnStopMoving += MoveAction_OnStopMoving;
		}

		if(TryGetComponent<ShootAction>(out ShootAction shootAction))
		{
			shootAction.OnShoot += ShootAction_OnShoot;
		}
		
		if(TryGetComponent<MeleeAction>(out MeleeAction meleeAction))
		{
			meleeAction.OnMeleeStart += MeleeAction_OnMeleeStart;
			meleeAction.OnMeleeComplete += MeleeAction_OnMeleeComplete;
		}
	}

	private void MoveAction_OnStartMoving(object sender, EventArgs e)
	{
		unitAnimator.SetBool("isWalking", true);
	}

	private void MoveAction_OnStopMoving(object sender, EventArgs e)
	{
		unitAnimator.SetBool("isWalking", false);
	}

	private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
	{
		unitAnimator.SetTrigger("shoot");

		Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
		BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();
		Vector3 targetUnitPosition = e.targetUnit.GetWorldPosition();
		targetUnitPosition.y = shootPointTransform.position.y;
		bulletProjectile.Setup(targetUnitPosition);
	}

	private void MeleeAction_OnMeleeStart(object sender, EventArgs e)
	{
		unitAnimator.SetTrigger("melee");
	}
	
	private void MeleeAction_OnMeleeComplete(object sender, EventArgs e)
	{

	}
}