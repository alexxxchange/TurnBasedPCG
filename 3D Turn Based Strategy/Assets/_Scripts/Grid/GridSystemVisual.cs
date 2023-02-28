using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
	public static GridSystemVisual Instance { get; private set; }

	[Serializable]
	public struct GridVisualTypeMaterial
	{
		public GridVisualType gridVisualType;
		public Material material;
	}

	public enum GridVisualType
	{
		White,
		Red,
		Blue,
		Yellow,
		Purple,
		Orange,
		Green,
		Black,
	}

	[SerializeField]
	private Transform gridVisualSinglePrefab;
	[SerializeField]
	private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

	private GridVisualSingle[,] gridVisualSingleArray;


	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("More than one GridSystemVisual! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		gridVisualSingleArray = new GridVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];

		for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
		{
			for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
			{
				GridPosition gridPosition = new GridPosition(x, z);
				Transform gridVisualSingleTransform = Instantiate(gridVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
				gridVisualSingleArray[x, z] = gridVisualSingleTransform.GetComponent<GridVisualSingle>();
			}
		}

		UpdateGridVisual();

		UnitActionSystem.Instance.OnChangeSelectedAction += UnitActionSystem_OnChangeSelectedAction;
		LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
	}
	public void HideAllGridPosition()
	{
		foreach (GridVisualSingle gridVisualSingle in gridVisualSingleArray)
		{
			gridVisualSingle.Hide();
		}
	}

	private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
	{
		List<GridPosition> gridPositionList = new List<GridPosition>();

		for (int x = -range; x <= range; x++)
		{
			for (int z = -range; z <= range; z++)
			{
				GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

				if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
				{
					continue;
				}

				int testDistance = (x * x) + (z * z);
				if (testDistance > (range * range))
				{
					continue;
				}

				gridPositionList.Add(testGridPosition);
			}
		}

		ShowGridPositionList(gridPositionList, gridVisualType);
	}


	public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
	{
		foreach (GridPosition gridPosition in gridPositionList)
		{
			gridVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
		}
	}

	/// <summary>
	/// 
	///	SET GRID SYSTEM VISUAL TYPES FOR ACTIONS
	/// 
	/// </summary>

	private void UpdateGridVisual()
	{
		HideAllGridPosition();

		Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
		BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
		GridVisualType gridVisualType;
		
		switch (selectedAction)
		{
			default:
			case MoveAction moveAction:
				gridVisualType = GridVisualType.White;
				break;
			case SpinAction spinAction:
				gridVisualType = GridVisualType.Green;
				break;
			case ShootAction shootAction:
				gridVisualType = GridVisualType.Red;
				ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetRange(), GridVisualType.Yellow);
				break;
			case GrenadeAction grenadeAction:
				gridVisualType = GridVisualType.Orange;
				break;
			case MeleeAction meleeAction:
				gridVisualType = GridVisualType.Red;
				break;
			case InteractAction interactAction:
				gridVisualType = GridVisualType.Purple;
				break;

		}

		ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
	}

	private void UnitActionSystem_OnChangeSelectedAction(object sender, EventArgs e)
	{
		UpdateGridVisual();
	}

	private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
	{
		UpdateGridVisual();
	}

	public Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
	{
		foreach(GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
		{
			if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
			{
				return gridVisualTypeMaterial.material;
			}
		}

		Debug.LogError("No assigned material for GridVisualType: " + gridVisualType);
		return null;
	}
}
