using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedVisual : MonoBehaviour
{
	[SerializeField]
	private Unit unit;

	private MeshRenderer meshRenderer;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Start()
	{
		UnitActionSystem.Instance.OnChangeSelectedUnit += UnitActionSystem_OnChangeSelectedUnit;
		UpdateSelectedVisual();
	}

	private void UnitActionSystem_OnChangeSelectedUnit(object sender, EventArgs empty)
	{
		UpdateSelectedVisual();
	}

	private void UpdateSelectedVisual()
	{
		if(UnitActionSystem.Instance.GetSelectedUnit() != unit)
		{
			meshRenderer.enabled = false;
		}
		else
		{
			meshRenderer.enabled = true;
		}
	}

	private void OnDestroy()
	{
		UnitActionSystem.Instance.OnChangeSelectedUnit -= UnitActionSystem_OnChangeSelectedUnit;
	}
}
