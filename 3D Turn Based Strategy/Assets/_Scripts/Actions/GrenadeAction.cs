using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
	[SerializeField]
	private Transform grenadePrefab;

	[SerializeField]
	private int actionPointsCost = 1;
	protected override int ActionPointsCost => actionPointsCost;

	[SerializeField]
	private int range = 5;
	[SerializeField]
	private LayerMask obstacleLayerMask;

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
	}

	public override string GetActionName()
	{
		return "Grenade";
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		return new EnemyAIAction
		{
			gridPosition = gridPosition,
			actionValue = 1,
		};
	}

	public override List<GridPosition> GetValidActionGridPositionList()
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();
		GridPosition unitGridPosition = unit.GetGridPosition();

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
				if (testDistance > (range * range))
				{
					continue;
				}

				Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
				Vector3 unitPosition = unitWorldPosition + Vector3.up * CameraManager.Instance.GetUnitShoulderHeight();
				Vector3 targetDirection = (LevelGrid.Instance.GetWorldPosition(testGridPosition) - unitWorldPosition).normalized;
				float targetDistance = Vector3.Distance(unitWorldPosition, LevelGrid.Instance.GetWorldPosition(testGridPosition));
				if (Physics.Raycast(unitPosition, targetDirection, targetDistance, obstacleLayerMask))
				{
					continue;
				}

				if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
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
		Transform grenadeTransform = Instantiate(grenadePrefab, unit.GetWorldPosition(), Quaternion.identity);
		Grenade grenade = grenadeTransform.GetComponent<Grenade>();
		grenade.Setup(gridPosition, OnGrenadeBehaviourComplete);
		ActionStart(onActionComplete);
	}

	private void OnGrenadeBehaviourComplete()
	{
		ActionComplete();
	}
}
