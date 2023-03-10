using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private CinemachineVirtualCamera cinemachineVirtualCamera;
	[SerializeField]
	private float cameraMoveSpeed;
	[SerializeField]
	private float cameraRotationSpeed;
	[SerializeField]
	private float zoomSpeed = 5f; // not used in ResetDefaultZoomLevel() for snappier response
	[SerializeField]
	private int defaultZoomLevel = 9;
	[SerializeField]
	private float zoomAngleFactor = 0.65f; // adjusts the slope/curve of the zoom motion. [ Y (camera height) = Z (camera distance) * zoomAngleFactor ] DO NOT set above 1.
	[SerializeField]
	private float CURR_ZOOM_LEVEL; // is set to match defaultZoomLevel in ResetDefaultZoomLevel(), which is called on Start() 
	[SerializeField]
	private int MIN_CAM_ZOOM = 3;
	[SerializeField]
	private int MAX_CAM_ZOOM = 15;

	private CinemachineTransposer cinemachineTransposer;
	private Vector3 targetFollowOffset;

	private void Start()
	{
		cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
		targetFollowOffset = cinemachineTransposer.m_FollowOffset;

		ResetDefaultZoomLevel();
	}


	private void Update()
	{
		HandleMovement();
		HandleRotation();
		HandleZoom();

		if (InputManager.Instance.GetMiddleMouseButtonDown())
		{
			ResetDefaultZoomLevel();
		}
	}

	private void HandleMovement()
	{
		Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();
		Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
		transform.position += moveVector * cameraMoveSpeed * Time.deltaTime;
	}

	private void HandleRotation()
	{
		Vector3 rotationVector = new Vector3(0, InputManager.Instance.GetCameraRotationDir(), 0);

		transform.eulerAngles += rotationVector * cameraRotationSpeed * Time.deltaTime;
	}

	private void HandleZoom()
	{
		CURR_ZOOM_LEVEL += InputManager.Instance.GetMouseScrollDelta();
		CURR_ZOOM_LEVEL = Mathf.Clamp(CURR_ZOOM_LEVEL, MIN_CAM_ZOOM, MAX_CAM_ZOOM);
		targetFollowOffset.z = -CURR_ZOOM_LEVEL;
		targetFollowOffset.y = CURR_ZOOM_LEVEL * zoomAngleFactor;
		cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
	}

	private void ResetDefaultZoomLevel()
	{
		targetFollowOffset.z = -defaultZoomLevel;
		targetFollowOffset.y = defaultZoomLevel * zoomAngleFactor;
		CURR_ZOOM_LEVEL = defaultZoomLevel;
		cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime);
	}
}
