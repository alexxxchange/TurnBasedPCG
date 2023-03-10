using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
	[SerializeField]
	private Material redMaterial;
	[SerializeField]
	private Material greenMaterial;
	[SerializeField]
	private MeshRenderer meshRenderer;
	private bool isRed;
	private Action onInteractComplete;
	private bool isActive;
	private float timer;
	private GridPosition gridPosition;

	private void Start()
	{
		gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
		Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
		SetColorRed();
	}

	private void Update()
	{
		if (!isActive) return;

		timer -= Time.deltaTime;

		if (timer <= 0f)
		{
			isActive = false;
			onInteractComplete();
		}
	}

	private void SetColorGreen()
	{
		isRed = false;
		meshRenderer.material = greenMaterial;
	}

	private void SetColorRed()
	{
		isRed = true;
		meshRenderer.material = redMaterial;
	}

	public void Interact(Action onInteractComplete)
	{
		this.onInteractComplete = onInteractComplete;
		isActive = true;
		timer = .25f;

		if (isRed)
		{
			SetColorGreen();
		}
		else
		{
			SetColorRed();
		}
	}
}
