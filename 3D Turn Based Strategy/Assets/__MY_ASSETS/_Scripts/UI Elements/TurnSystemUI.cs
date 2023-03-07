using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSystemUI : MonoBehaviour
{
	[SerializeField]
	private Button endTurnButton;
	[SerializeField]
	private TextMeshProUGUI turnNumberText;
	[SerializeField]
	private GameObject enemyTurnVisual;

	private void Start()
	{
		endTurnButton.onClick.AddListener(() => { TurnSystem.Instance.NextTurn(); });
		TurnSystem.Instance.OnNextTurn += TurnSystem_OnNextTurn;
		UpdateTurnNumberText();
		UpdateEnemyTurnVisual();
		UpdateEndTurnButtonVisibility();
	}

	private void TurnSystem_OnNextTurn(object sender, EventArgs e)
	{
		UpdateTurnNumberText();
		UpdateEnemyTurnVisual();
		UpdateEndTurnButtonVisibility();
	}
	
	private void UpdateTurnNumberText()
	{
		turnNumberText.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
	}

	private void UpdateEnemyTurnVisual()
	{
		enemyTurnVisual.SetActive(!TurnSystem.Instance.IsPlayerTurn());
	}

	private void UpdateEndTurnButtonVisibility()
	{
		endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
	}
}
