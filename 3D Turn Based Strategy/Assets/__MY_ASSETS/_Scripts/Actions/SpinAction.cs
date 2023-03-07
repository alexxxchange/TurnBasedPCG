using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
	[SerializeField]
	private int actionPointsCost;
	protected override int ActionPointsCost { get => actionPointsCost; }

	private float totalSpinAmount = 0f;

	private void Update()
	{
		int ap = unit.GetActionPoints();
		if (ap > 0)
		{
			actionPointsCost = ap;
		}
		
		if (!isActive)
		{
			return;
		}

		float spinAddAmount = 360f * Time.deltaTime;
		transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

		totalSpinAmount += spinAddAmount;
		if(totalSpinAmount >= 360f)
		{
			ActionComplete();
		}
	}

	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		totalSpinAmount = 0f;
		ActionStart(onActionComplete);
	}
	public override string GetActionName()
	{
		return "Spin";
	}

	public override List<GridPosition> GetValidActionGridPositionList()
	{
		GridPosition unitGridPosition = unit.GetGridPosition();

		return new List<GridPosition> { unitGridPosition };
	}

	public override int GetActionPointsCost()
	{
		return ActionPointsCost;
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		return new EnemyAIAction
		{
			gridPosition = gridPosition,
			actionValue = 0,
		};
	}
}
