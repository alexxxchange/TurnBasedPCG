using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualSingle : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	public void Hide()
	{
		meshRenderer.enabled = false;
	}

	public void Show(Material material)
	{
		meshRenderer.enabled = true;
		meshRenderer.material = material;
	}
}
