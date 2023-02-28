using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
	private GridPosition gridPosition;
	private Animator animator;
	private Action onInteractComplete;
	private bool isActive;
	private float timer;
	
	[SerializeField]
	private bool isOpen;


	private void Awake()
	{
		animator = GetComponent<Animator>();
	}
	private void Start()
	{
		gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
		animator.speed = 200f;
		if (isOpen)
		{
			OpenDoor();
		}
		else
		{
			CloseDoor();
		}
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

	public void Interact(Action onInteractComplete)
	{
		animator.speed = 1f;
		this.onInteractComplete = onInteractComplete;
		isActive = true;
		timer = .5f;
		if (isOpen)
		{
			CloseDoor();
		}
		else
		{
			OpenDoor();
		}
	}

	private void OpenDoor()
	{
		isOpen = true;
		animator.SetBool("isOpen", isOpen);
		Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
	}

	private void CloseDoor()
	{
		isOpen = false;
		animator.SetBool("isOpen", isOpen);
		Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
	}
}
