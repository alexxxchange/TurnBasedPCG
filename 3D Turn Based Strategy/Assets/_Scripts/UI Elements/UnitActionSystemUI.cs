using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
	[SerializeField]
	private Transform actionButtonPrefab;
	[SerializeField]
	private Transform actionButtonContainer;
	[SerializeField]
	private TextMeshProUGUI actionPointsText;

	private List<ActionButtonUI> actionButtonUIList;

	private void Awake()
	{
		actionButtonUIList = new List<ActionButtonUI>();
	}

	private void Start()
	{
		UnitActionSystem.Instance.OnChangeSelectedUnit += UnitActionSystem_OnChangeSelectedUnit;
		UnitActionSystem.Instance.OnChangeSelectedAction += UnitActionSystem_OnChangeSelectedAction;
		UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
		TurnSystem.Instance.OnNextTurn += TurnSystem_OnNextTurn;
		Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

		CreateUnitActionButtons();
		UpdateSelectedVisual();
		UpdateActionPoints();
	}

	private void CreateUnitActionButtons()
	{
		foreach(Transform button in actionButtonContainer)
		{
			Destroy(button.gameObject);
		}

		actionButtonUIList.Clear();

		Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

		foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
		{
			Transform actionButton = Instantiate(actionButtonPrefab, actionButtonContainer);
			ActionButtonUI actionButtonUI = actionButton.GetComponent<ActionButtonUI>();
			actionButtonUI.SetBaseAction(baseAction);
			actionButtonUIList.Add(actionButtonUI);
		}

	}

	private void UnitActionSystem_OnChangeSelectedUnit(object sender, EventArgs e)
	{
		CreateUnitActionButtons();
		UpdateSelectedVisual();
		UpdateActionPoints();
	}

	private void UnitActionSystem_OnChangeSelectedAction(object sender, EventArgs e)
	{
		UpdateSelectedVisual();
	}

	private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
	{
		UpdateActionPoints();
	}

	private void TurnSystem_OnNextTurn(object sender, EventArgs e)
	{
		UpdateActionPoints();
	}

	private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
	{
		UpdateActionPoints();
	}

	private void UpdateSelectedVisual()
	{
		foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
		{
			actionButtonUI.UpdateSelectedVisual();
		}
	}

	private void UpdateActionPoints()
	{
		Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
		actionPointsText.text = "Action Points : " + selectedUnit.GetActionPoints();
	}

}
