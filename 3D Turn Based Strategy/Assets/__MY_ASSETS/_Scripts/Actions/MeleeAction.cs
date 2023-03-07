using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAction : BaseAction
{
	public event EventHandler OnMeleeStart;
	public event EventHandler OnMeleeComplete;

	[SerializeField]
	private int actionPointsCost = 1;
	protected override int ActionPointsCost => actionPointsCost;

	[SerializeField]
	private int range = 1;
	[SerializeField]
	private float rotationSpeed = 10f;
	[SerializeField]
	private int damageAmount = 100;

	private Unit targetUnit;

	private enum State
	{
		SwingBeforeHit,
		SwingAfterHit,
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
			case State.SwingBeforeHit:
				Vector3 targetDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
				transform.forward = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * rotationSpeed);
				break;
			case State.SwingAfterHit:
				break;
		}

		if (stateTimer <= 0)
		{
			NextState();
		}
	}

	private void NextState()
	{
		switch (state)
		{
			case State.SwingBeforeHit:
				state = State.SwingAfterHit;
				float afterHitStateTime = 0.5f;
				stateTimer = afterHitStateTime;
				targetUnit.Damage(damageAmount);
				break;
			case State.SwingAfterHit:
				OnMeleeComplete?.Invoke(this, EventArgs.Empty);
				ActionComplete();
				break;
		}
	}

	public override string GetActionName()
	{
		return "Melee";
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

				if (!LevelGrid.Instance.IsOccupiedGridPosition(testGridPosition))
				{
					continue;
				}

				Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
				if (targetUnit.IsEnemy() == unit.IsEnemy())
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

		state = State.SwingBeforeHit;
		float beforeHitStateTime = 0.7f;
		stateTimer = beforeHitStateTime;

		OnMeleeStart?.Invoke(this, EventArgs.Empty);
		ActionStart(onActionComplete);
	}
}
