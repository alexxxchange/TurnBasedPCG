using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
	[SerializeField]
	private int range = 1;

	[SerializeField]
	private int actionPointsCost = 1;
	protected override int ActionPointsCost => actionPointsCost;

	private Doorway targetDoor;

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
	}

	public override string GetActionName()
	{
		return "Interact";
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		return new EnemyAIAction
		{
			gridPosition = gridPosition,
			actionValue = 0,
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

				IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);

				if (interactable == null)
				{
					continue;
				}

				if (interactable != null && LevelGrid.Instance.IsOccupiedGridPosition(testGridPosition))
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
		IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);

		interactable.Interact(OnInteractComplete);

		// OnInteractStart?.Invoke(this, EventArgs.Empty);
		ActionStart(onActionComplete);
	}

	private void OnInteractComplete()
	{
		ActionComplete();
	}
}
