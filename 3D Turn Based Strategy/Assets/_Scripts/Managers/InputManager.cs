#define USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	public static InputManager Instance { get; private set; }

	private PlayerInputActions playerInputActions;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("More than one InputManager! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		Instance = this;

		playerInputActions = new PlayerInputActions();
		playerInputActions.Player.Enable();
	}

	public Vector2 GetMouseScreenPosition()
	{

		return Mouse.current.position.ReadValue();

	}

	public bool GetLeftMouseButtonDown()
	{

		return playerInputActions.Player.LeftClick.WasPressedThisFrame();

	}

	public bool GetRightMouseButtonDown()
	{

		return playerInputActions.Player.RightClick.WasPressedThisFrame();

	}

	public bool GetMiddleMouseButtonDown()
	{

		return playerInputActions.Player.MiddleClick.WasPressedThisFrame();

	}

	public Vector2 GetCameraMoveVector()
	{

		return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();

	}

	public float GetCameraRotationDir()
	{

		return playerInputActions.Player.CameraRotation.ReadValue<float>();

	}

	public float GetMouseScrollDelta()
	{

		return playerInputActions.Player.CameraZoom.ReadValue<float>();

	}
}
