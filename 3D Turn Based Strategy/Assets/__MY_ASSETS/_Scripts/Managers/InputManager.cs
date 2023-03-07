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
#if USE_NEW_INPUT_SYSTEM
		return Mouse.current.position.ReadValue();
#else
		return Input.mousePosition;
#endif
	}

	public bool GetLeftMouseButtonDown()
	{
#if USE_NEW_INPUT_SYSTEM
		return playerInputActions.Player.LeftClick.WasPressedThisFrame();
#else
		return Input.GetMouseButtonDown(0);
#endif
	}

	public bool GetRightMouseButtonDown()
	{
#if USE_NEW_INPUT_SYSTEM
		return playerInputActions.Player.RightClick.WasPressedThisFrame();
#else
		return Input.GetMouseButtonDown(1);
#endif
	}

	public bool GetMiddleMouseButtonDown()
	{
#if USE_NEW_INPUT_SYSTEM
		return playerInputActions.Player.MiddleClick.WasPressedThisFrame();
#else
		return Input.GetMouseButtonDown(2);
#endif
	}

	public Vector2 GetCameraMoveVector()
	{
#if USE_NEW_INPUT_SYSTEM
		return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
		Vector2 inputMoveDir = new Vector2(0, 0);

		if (Input.GetKey(KeyCode.W))
		{
			inputMoveDir.y = +1f;
		}
		if (Input.GetKey(KeyCode.S))
		{
			inputMoveDir.y = -1f;
		}
		if (Input.GetKey(KeyCode.A))
		{
			inputMoveDir.x = -1f;
		}
		if (Input.GetKey(KeyCode.D))
		{
			inputMoveDir.x = +1f;
		}

		return inputMoveDir;
#endif
	}

	public float GetCameraRotationDir()
	{
#if USE_NEW_INPUT_SYSTEM
		return playerInputActions.Player.CameraRotation.ReadValue<float>();
#else
		float rotationVector = 0f;

		if (Input.GetKey(KeyCode.Q))
		{
			rotationVector = -1f;
		}
		if (Input.GetKey(KeyCode.E))
		{
			rotationVector = +1f;
		}

		return rotationVector;
#endif
	}

	public float GetMouseScrollDelta()
	{
#if USE_NEW_INPUT_SYSTEM
		return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
		float zoomIncrement = 0f;

		if (Input.mouseScrollDelta.y > 0)
		{
			zoomIncrement -= 1f;
		}
		if (Input.mouseScrollDelta.y < 0)
		{
			zoomIncrement += 1f;
		}

		return zoomIncrement;
#endif
	}
}
