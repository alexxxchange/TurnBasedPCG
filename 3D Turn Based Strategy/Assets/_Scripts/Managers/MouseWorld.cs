using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
	private static MouseWorld Instance;

	[SerializeField]
	private LayerMask mouseWorldPlaneLayerMask;

	private void Awake()
	{
		Instance = this;
	}

	void Update()
	{
		transform.position = MouseWorld.GetPosition();
     }

	public static Vector3 GetPosition()
	{
		Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
		Physics.Raycast(ray, out RaycastHit rayCastHit, float.MaxValue, Instance.mouseWorldPlaneLayerMask);

		return rayCastHit.point;
	}
}
