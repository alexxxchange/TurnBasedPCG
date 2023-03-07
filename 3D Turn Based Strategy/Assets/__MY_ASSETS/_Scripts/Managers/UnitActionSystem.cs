using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
	public static UnitActionSystem Instance { get; private set; }

	public event EventHandler OnChangeSelectedUnit;
	public event EventHandler OnChangeSelectedAction;
	public event EventHandler OnSetActionBusy;
	public event EventHandler OnUnsetActionBusy;
	public event EventHandler OnActionStarted;

	[SerializeField]
	private Unit selectedUnit;

	private BaseAction selectedAction;

	[SerializeField]
	private LayerMask unitLayerMask;

	private bool isBusy;

	private void Awake()
	{
		if(Instance != null)
		{
			Debug.LogError("More than one UnitActionSystem! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		if(selectedUnit != null)
		{
			SetSelectedUnit(selectedUnit);
		}
	}

	private void Update()
	{
		if (isBusy)
		{
			return;
		}

		if (!TurnSystem.Instance.IsPlayerTurn())
		{
			return;
		}

		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}

		HandleSelectedAction();
	}

	private void HandleSelectedAction()
	{
		if (InputManager.Instance.GetLeftMouseButtonDown())
		{
			SelectUnit();
		}

		if (InputManager.Instance.GetRightMouseButtonDown())
		{
			GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
			if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
			{
				Debug.Log("action failure!");
				return;
			}

			if (selectedUnit.TrySpendActionPoints(selectedAction.GetActionPointsCost()))
			{
				SetBusy();
				selectedAction.TakeAction(mouseGridPosition, ClearBusy);
				OnActionStarted?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	private void SelectUnit()
	{
		Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
		if(Physics.Raycast(ray, out RaycastHit rayCastHit, float.MaxValue, unitLayerMask))
		{
			if (rayCastHit.transform.TryGetComponent<Unit>(out Unit unit))
			{
				if (unit == selectedUnit || unit.IsEnemy())
				{
					return;
				}

				SetSelectedUnit(unit);
			}
		}
	}

	public void SetSelectedUnit(Unit unit)
	{
		selectedUnit = unit;
		SetSelectedAction(unit.GetDefaultAction());
		OnChangeSelectedUnit?.Invoke(this, EventArgs.Empty);
	}

	public void SetSelectedAction(BaseAction baseAction)
	{
		selectedAction = baseAction;
		OnChangeSelectedAction?.Invoke(this, EventArgs.Empty);
	}

	public Unit GetSelectedUnit()
	{
		return selectedUnit;
	}
	public BaseAction GetSelectedAction()
	{
		return selectedAction;
	}

	private void SetBusy()
	{
		isBusy = true;
		OnSetActionBusy?.Invoke(this, EventArgs.Empty);
	}

	private void ClearBusy()
	{
		isBusy = false;
		OnUnsetActionBusy?.Invoke(this, EventArgs.Empty);
	}
}
