using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static CameraManager Instance { get; private set; }

	[SerializeField]
	private GameObject actionCameraGameObject;
	[SerializeField]
	private const float UNIT_SHOULDER_HEIGHT = 1.7f;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("More than one CameraManager! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
		BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

		HideActionCamera();
	}

	private void ShowActionCamera()
	{
		actionCameraGameObject.SetActive(true);
	}


	private void HideActionCamera()
	{
		actionCameraGameObject.SetActive(false);
	}

	private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
	{
		switch (sender)
		{
			case ShootAction shootAction:
				SetActionCameraPosition(shootAction);
				ShowActionCamera();

				break;
		}
	}

	private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
	{
		switch (sender)
		{
			case ShootAction shootAction:

				HideActionCamera();

				break;
		}
	}

	private void SetActionCameraPosition(ShootAction shootAction)
	{
		Unit shooterUnit = shootAction.GetUnit();
		Unit targetUnit = shootAction.GetTargetUnit();

		Vector3 cameraCharacterHeight = Vector3.up * UNIT_SHOULDER_HEIGHT;

		Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

		float shoulderOffsetAmount = 0.5f;
		Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

		Vector3 actionCameraPosition =
		    shooterUnit.GetWorldPosition() +
		    cameraCharacterHeight +
		    shoulderOffset +
		    (shootDir * -1);

		actionCameraGameObject.transform.position = actionCameraPosition;
		actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);

	}

	public float GetUnitShoulderHeight()
	{
		return UNIT_SHOULDER_HEIGHT;
	}
}
