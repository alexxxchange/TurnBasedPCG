using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
	[SerializeField]
	private int INIT_ACTION_POINTS = 4;

	public static event EventHandler OnAnyActionPointsChanged;
	public static event EventHandler OnAnyUnitSpawned;
	public static event EventHandler OnAnyUnitDead;


	private GridPosition gridPosition;
	private BaseAction[] baseActionArray;
	private HealthSystem healthSystem;
	
	[SerializeField]
	private int actionPoints;
	[SerializeField]
	private bool isEnemy;


	private void Awake()
	{
		healthSystem = GetComponent<HealthSystem>();
		baseActionArray = GetComponents<BaseAction>();
	}

	private void Start()
	{
		gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
		TurnSystem.Instance.OnNextTurn += TurnSystem_OnNextTurn;
		healthSystem.OnDead += HealthSystem_OnDead;
		OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
		actionPoints = INIT_ACTION_POINTS;
	}

	private void Update()
	{
		GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		if (newGridPosition != gridPosition)
		{
			GridPosition oldGridPosition = gridPosition;
			gridPosition = newGridPosition;
			LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
		}
	}

	public BaseAction GetDefaultAction()
	{
		return baseActionArray[0];
	}

	public GridPosition GetGridPosition()
	{
		return gridPosition;
	}

	public Vector3 GetWorldPosition()
	{
		return transform.position;
	}

	public BaseAction[] GetBaseActionArray()
	{
		return baseActionArray;
	}

	public bool TrySpendActionPoints(int points)
	{
		if (CanAffordAction(points))
		{
			SpendActionPoints(points);
			return true;
		}
		else
		{
			return false;
		}

	}

	public bool TrySpendActionPoints(BaseAction action)
	{
		int ap = action.GetActionPointsCost();

		if (CanAffordAction(ap))
		{
			SpendActionPoints(ap);
			return true;
		}
		else
		{
			return false;
		}

	}

	private bool CanAffordAction(int points)
	{
		return actionPoints >= points;
	}

	public bool CanAffordAction(BaseAction action)
	{
		return actionPoints >= action.GetActionPointsCost();
	}

	private void SpendActionPoints(int points)
	{
		actionPoints -= points;
		OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
	}

	public int GetActionPoints()
	{
		return actionPoints;
	}

	private void TurnSystem_OnNextTurn(object sender, EventArgs e)
	{
		if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
		{
			actionPoints = INIT_ACTION_POINTS;
			OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void HealthSystem_OnDead(object sender, EventArgs e)
	{
		LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
		Destroy(gameObject);
		OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
	}

	public bool IsEnemy() {
		return isEnemy;
	}

	public void Damage(int damageAmount)
	{
		healthSystem.Damage(damageAmount);
	}

	public float GetHealthNormalized()
	{
		return healthSystem.GetHealthNormalized();
	}
}
