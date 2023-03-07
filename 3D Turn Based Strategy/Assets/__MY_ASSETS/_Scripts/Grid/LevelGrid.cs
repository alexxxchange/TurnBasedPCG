using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
	public event EventHandler OnAnyUnitMovedGridPosition;

	public static LevelGrid Instance { get; private set; }


	[SerializeField]
	private int levelGridWidth = 10;
	[SerializeField]
	private int levelGridHeight = 10;
	[SerializeField]
	private float levelGridCellSize = 2f;
	[SerializeField]
	private Transform gridDebugObjectPrefab;

	private GridSystem<GridObject> gridSystem;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("More than one LevelGrid! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		Instance = this;

		gridSystem = new GridSystem<GridObject>(levelGridWidth, levelGridHeight, levelGridCellSize,
			(GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
		// gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

	}

	private void Start()
	{
		// Pathfinding.Instance.Setup(levelGridWidth, levelGridHeight, levelGridCellSize);
	}

	public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		gridObject.AddUnit(unit);
	}

	public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		return gridObject.GetUnitList();
	}

	public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		gridObject.RemoveUnit(unit);
	}

	public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
	{
		RemoveUnitAtGridPosition(fromGridPosition, unit);
		AddUnitAtGridPosition(toGridPosition, unit);

		OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
	}

	public GridPosition GetGridPosition(Vector3 worldPosition)
	{
		return gridSystem.GetGridPosition(worldPosition);
	}

	public Vector3 GetWorldPosition(GridPosition gridPosition)
	{
		return gridSystem.GetWorldPosition(gridPosition);
	}

	public bool IsValidGridPosition(GridPosition gridPosition)
	{
		return gridSystem.IsValidGridPosition(gridPosition);
	}

	public bool IsOccupiedGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		return gridObject.IsOccupied();
	}

	public int GetWidth()
	{
		return gridSystem.GetWidth();
	}

	public int GetHeight()
	{
		return gridSystem.GetHeight();
	}

	public Unit GetUnitAtGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		return gridObject.GetUnit();
	}

	public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		return gridObject.GetInteractable();
	}
	
	public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		gridObject.SetInteractable(interactable);
	}

	internal float GetCellSize()
	{
		return levelGridCellSize;
	}
}
