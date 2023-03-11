using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
	public static TurnSystem Instance { get; private set; }

	public event EventHandler OnNextTurn;

	private int turnNumber = 1;
	private bool isPlayerTurn = true;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("More than one TurnSystem! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public void NextTurn()
	{
		turnNumber++;
		isPlayerTurn = !isPlayerTurn;
		List<Unit> friendlyUnits = UnitManager.Instance.GetFriendlyUnitList();
		if (isPlayerTurn && friendlyUnits.Count > 0)
		{
			Unit nextUnit = friendlyUnits[0];
			UnitActionSystem.Instance.SetSelectedUnit(nextUnit);
		}

		OnNextTurn?.Invoke(this, EventArgs.Empty);
	}

	public int GetTurnNumber()
	{
		return turnNumber;
	}

	public bool IsPlayerTurn()
	{
		return isPlayerTurn;
	}
}
