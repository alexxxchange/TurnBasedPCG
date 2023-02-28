using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
	public event EventHandler<OnShootEventArgs> OnShoot;
	public static event EventHandler<OnShootEventArgs> OnAnyShoot;

	public class OnShootEventArgs : EventArgs
	{
		public Unit targetUnit;
		public Unit shootingUnit;
	}

	[SerializeField]
	private int actionPointsCost = 1;
	protected override int ActionPointsCost { get => actionPointsCost; }
	[SerializeField]
	private int range = 5;
	[SerializeField]
	private LayerMask obstacleLayerMask;
	[SerializeField]
	private float rotationSpeed = 10f;
	[SerializeField]
	private int damageAmount = 40;

	private Unit targetUnit;
	private bool canShoot;
	
	private enum State
	{
		Aiming,
		Shooting,
		Cooldown,
	}

	private State state;
	private float stateTimer;

	private void Update()
	{
		if (!isActive)
		{
			return;
		}

		stateTimer -= Time.deltaTime;

		switch (state)
		{
			case State.Aiming:
				Vector3 targetDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
				transform.forward = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * rotationSpeed);
				break;
			case State.Shooting:
				if (canShoot)
				{
					Shoot();
					canShoot = false;
				}
				break;
			case State.Cooldown:
				break;
		}

		if(stateTimer <= 0)
		{
			NextState();
		}

	}

	private void NextState()
	{
		switch (state)
		{
			case State.Aiming:
				state = State.Shooting;
				float shootingStateTime = 0.1f;
				stateTimer = shootingStateTime;
				break;
			case State.Shooting:
				state = State.Cooldown;
				float cooldownStateTime = 0.5f;
				stateTimer = cooldownStateTime;
				break;
			case State.Cooldown:
				ActionComplete();
				break;
		}
	}


	public override string GetActionName()
	{
		return "Shoot";
	}

	public override List<GridPosition> GetValidActionGridPositionList()
	{
		GridPosition unitGridPosition = unit.GetGridPosition();
		return GetValidActionGridPositionList(unitGridPosition);
	}

	public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();

		for (int x = -range; x <= range; x++)
		{
			for (int z = -range; z <= range; z++)
			{
				GridPosition offsetGridPosition = new GridPosition(x, z);
				GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

				if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
				{
					continue;
				}

				int testDistance = (x * x) + (z * z);
				if(testDistance > (range * range))
				{
					continue;
				}

				if (!LevelGrid.Instance.IsOccupiedGridPosition(testGridPosition))
				{
					continue;
				}

				Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
				if (targetUnit.IsEnemy() == unit.IsEnemy())
				{
					continue;
				}

				Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
				Vector3 unitPosition = unitWorldPosition + Vector3.up * CameraManager.Instance.GetUnitShoulderHeight();
				Vector3 shootDirection = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
				float shootDistance = Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition());
				if (Physics.Raycast(unitPosition, shootDirection, shootDistance, obstacleLayerMask))
				{
					continue;
				}

				validGridPositionList.Add(testGridPosition);
			}
		}

		return validGridPositionList;
	}

	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

		state = State.Aiming;
		float aimingStateTime = 1f;
		stateTimer = aimingStateTime;
		
		canShoot = true;

		ActionStart(onActionComplete);
	}

	private void Shoot()
	{
		OnAnyShoot?.Invoke(this, new OnShootEventArgs
		{
			targetUnit = targetUnit,
			shootingUnit = unit
		});

		OnShoot?.Invoke(this, new OnShootEventArgs
		{
			targetUnit = targetUnit,
			shootingUnit = unit
		});
		
		targetUnit.Damage(damageAmount);
	}

	public Unit GetTargetUnit()
	{
		return targetUnit;
	}

	public int GetRange()
	{
		return range;
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
		return new EnemyAIAction
		{
			gridPosition = gridPosition,
			actionValue = 100 + Mathf.RoundToInt((1-targetUnit.GetHealthNormalized()) * 100f),
		};
	}

	public int GetTargetCountAtPosition(GridPosition gridPosition)
	{
		return GetValidActionGridPositionList(gridPosition).Count;
	}
}
