using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
	private void OnDestroy()
	{
		GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
	}
}
