using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
	public static Pathfinding Instance { get; private set; }

	private const int MOVE_STRIGHT_COST = 10;
	private const int MOVE_DIAGONAL_COST = 14;

	[Header ("Width, height, and cellSize should match the LevelGrid!")]
	[SerializeField]
	private int width = 10;
	[SerializeField]
	private int height = 10;
	[SerializeField]
	private float cellSize = 2f;

	[SerializeField]
	private Transform gridDebugObjectprefab;
	[SerializeField]
	private LayerMask obstacleLayerMask;

	private GridSystem<PathNode> gridSystem;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("More than one Pathfinding! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public void Setup(int w, int h, float cs)
	{
		this.width = w;
		this.height = h;
		this.cellSize = cs;

		gridSystem = new GridSystem<PathNode>(width, height, cellSize, (GridSystem<PathNode> g, GridPosition gridposition) => new PathNode(gridposition));
		
		// gridSystem.CreateDebugObjects(gridDebugObjectprefab);

		for (int x = 0; x < width; x++)
		{
			for (int z = 0; z < height; z++)
			{
				GridPosition gridPosition = new GridPosition(x, z);
				Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
				float rayCastOffset = 5f;
				if(Physics.Raycast(worldPosition + Vector3.down * rayCastOffset, Vector3.up, rayCastOffset * 2, obstacleLayerMask))
				{
					GetNode(x, z).SetIsWalkable(false);
				}


			}
		}
	}

	public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
	{
		List<PathNode> openList = new List<PathNode>();
		List<PathNode> closedList = new List<PathNode>();

		PathNode startNode = gridSystem.GetGridObject(startGridPosition);
		PathNode endNode = gridSystem.GetGridObject(endGridPosition);
		openList.Add(startNode);

		for (int x = 0; x < gridSystem.GetWidth(); x++)
		{
			for (int z = 0; z < gridSystem.GetHeight(); z++)
			{
				GridPosition gridPosition = new GridPosition(x, z);
				PathNode pathNode = gridSystem.GetGridObject(gridPosition);

				pathNode.SetGCost(int.MaxValue);
				pathNode.SetHCost(0);
				pathNode.CalculateFCost();
				pathNode.ResetPreviousNode();

			}
		}

		startNode.SetGCost(0);
		startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
		startNode.CalculateFCost();

		while (openList.Count > 0)
		{
			PathNode currentNode = GetLowestFCostPathNode(openList);

			if (currentNode == endNode)
			{
				pathLength = endNode.GetFCost();
				return CalculatePath(endNode);
			}

			openList.Remove(currentNode);
			closedList.Add(currentNode);

			foreach(PathNode neighborNode in GetNeighborList(currentNode))
			{
				if (closedList.Contains(neighborNode))
				{
					continue;
				}

				if (!neighborNode.IsWalkable())
				{
					closedList.Add(neighborNode);
					continue;
				}

				int tempGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighborNode.GetGridPosition());

				if(tempGCost < neighborNode.GetGCost())
				{
					neighborNode.SetGCost(tempGCost);
					neighborNode.SetHCost(CalculateDistance(neighborNode.GetGridPosition(), endGridPosition));
					neighborNode.CalculateFCost();
					neighborNode.SetPreviousNode(currentNode);

					if (!openList.Contains(neighborNode))
					{
						openList.Add(neighborNode);
					}
				}
			}
		}

		pathLength = 0;
		return null;
	}

	public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
	{
		GridPosition gridPositionDistance = gridPositionA - gridPositionB;
		int xDistance = Mathf.Abs(gridPositionDistance.x);
		int zDistance = Mathf.Abs(gridPositionDistance.z);
		int remaining = Mathf.Abs(xDistance - zDistance);
		return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRIGHT_COST * remaining;
	}

	private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
	{
		PathNode lowestFCostPathNode = pathNodeList[0];

		for(int i = 0; i < pathNodeList.Count; i++)
		{
			if(pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
			{
				lowestFCostPathNode = pathNodeList[i];
			}
		}
		return lowestFCostPathNode;

	}

	private PathNode GetNode(int x, int z)
	{
		return gridSystem.GetGridObject(new GridPosition(x, z));
	}

	private List<PathNode> GetNeighborList(PathNode currentNode)
	{
		List<PathNode> neighborList = new List<PathNode>();

		GridPosition gridPosition = currentNode.GetGridPosition();

		if (gridPosition.x - 1 >= 0)
		{
			neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0)); //Left

			if (gridPosition.z + 1 < gridSystem.GetHeight())
			{
				neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1)); // Upper Left
			}
			if (gridPosition.z - 1 >= 0)
			{
				neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1)); // Lower Left
			}

		}

		if (gridPosition.x + 1 < gridSystem.GetWidth())
		{
			neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0)); //Right

			if (gridPosition.z + 1 < gridSystem.GetHeight())
			{
				neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1)); // Upper Right
			}
			if (gridPosition.z - 1 >= 0)
			{
				neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1)); // Lower Right
			}

		}

		if (gridPosition.z + 1 < gridSystem.GetHeight())
		{
			neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1)); //Up
		}

		if (gridPosition.z - 1 >= 0)
		{
			neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1)); //Down
		}
			
		return neighborList;
	}

	private List<GridPosition> CalculatePath(PathNode endNode)
	{
		List<GridPosition> finalPath = new List<GridPosition>();
		finalPath.Add(endNode.GetGridPosition());
		PathNode currentNode = endNode;
		PathNode nextNode = currentNode.GetPreviousNode();
		
		while(nextNode != null)
		{
			currentNode = nextNode;
			finalPath.Add(currentNode.GetGridPosition());
			nextNode = currentNode.GetPreviousNode();
		}

		finalPath.Reverse();
		return finalPath;
	}

	public bool IsWalkableGridPosition(GridPosition gridPosition)
	{
		return gridSystem.GetGridObject(gridPosition).IsWalkable();
	}


	public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
	{
		gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
	}

	public bool HasPath(GridPosition start, GridPosition end)
	{
		return FindPath(start, end, out int pathLength) != null;
	}

	public int GetPathLength(GridPosition start, GridPosition end)
	{
		FindPath(start, end, out int pathLength);
		return pathLength;
	}
}
