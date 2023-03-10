using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
	public event EventHandler OnStartMoving;
	public event EventHandler OnStopMoving;
	
	[SerializeField]
	private int actionPointsCost = 1;
	protected override int ActionPointsCost { get => actionPointsCost; }
	[SerializeField]
	private int maxMoveDistance = 4;
	[SerializeField]
	private float moveSpeed = 4f;
	[SerializeField]
	private float rotationSpeed = 10f;
	[SerializeField]
	private float stopDistance = 0.1f;

	private List<Vector3> positionList;
	private int currentPositionIndex;
	
	private void Update()
	{
		if (!isActive)
		{
			return;
		}

		Vector3 targetPosition = positionList[currentPositionIndex];
		Vector3 moveDirection = (targetPosition - transform.position).normalized;
		
		// Rotate
		transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);

		if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
		{
			transform.position += moveDirection * moveSpeed * Time.deltaTime;
		}
		else
		{
			currentPositionIndex++;
			if (currentPositionIndex >= positionList.Count)
			{
				OnStopMoving?.Invoke(this, EventArgs.Empty);
				ActionComplete();
			}
		}
	}

	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
		currentPositionIndex = 0;
		positionList = new List<Vector3>();
		foreach(GridPosition pathGridPosition in pathGridPositionList)
		{
			positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
		}
		OnStartMoving?.Invoke(this, EventArgs.Empty);
		ActionStart(onActionComplete);
	}

	public override List<GridPosition> GetValidActionGridPositionList()
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();
		GridPosition unitGridPosition = unit.GetGridPosition();

		for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
		{
			for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
			{
				GridPosition offsetGridPosition = new GridPosition(x, z);
				GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

				if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
				{
					continue;
				}
				if (testGridPosition == unitGridPosition)
				{
					continue;
				}
				if (LevelGrid.Instance.IsOccupiedGridPosition(testGridPosition))
				{
					continue;
				}
				if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
				{
					continue;
				}
				if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
				{
					continue;
				}
				if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * 10) // NOT a magic number, is a CONSTANT in the Pathfinding algorithm
				{
					continue;
				}

				validGridPositionList.Add(testGridPosition);
			}
		}

		return validGridPositionList;
	}

	public override string GetActionName()
	{
		return "Move";
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		ShootAction shootAction = unit.GetBaseActionArray()[2] as ShootAction;

		return new EnemyAIAction
		{
			gridPosition = gridPosition,
			actionValue = shootAction.GetTargetCountAtPosition(gridPosition) * 10, // <- magic number BAD
		};
	}
}
