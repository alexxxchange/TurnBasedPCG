using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
	private GridPosition gridPosition;
	private int gCost;
	private int hCost;
	private int fCost;

	private PathNode previousNode;

	private bool isWalkable = true;

	
	public PathNode(GridPosition gridPosition)
	{
		this.gridPosition = gridPosition;
	}

	public GridPosition GetGridPosition()
	{
		return gridPosition;
	}

	public int GetGCost()
	{
		return gCost;
	}

	public int GetHCost()
	{
		return hCost;
	}

	public int GetFCost()
	{
		return fCost;
	}

	public PathNode GetPreviousNode()
	{
		return previousNode;
	}

	public void SetGCost(int value)
	{
		this.gCost = value;
	}

	public void SetHCost(int value)
	{
		this.hCost = value;
	}
	public void CalculateFCost()
	{
		fCost = gCost + hCost;
	}

	public void SetPreviousNode(PathNode pathNode)
	{
		previousNode = pathNode;
	}

	public void ResetPreviousNode()
	{
		previousNode = null;
	}

	public bool IsWalkable()
	{
		return isWalkable;
	}

	public void SetIsWalkable(bool isWalkable)
	{
		this.isWalkable = isWalkable;
	}

	public override string ToString()
	{
		return gridPosition.ToString();
	}
}
