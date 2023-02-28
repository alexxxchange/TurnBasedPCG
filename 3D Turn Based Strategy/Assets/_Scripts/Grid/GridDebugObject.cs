using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
	private object gridObject;

	[SerializeField]
	private TextMeshPro gridPositionText;

	public virtual void SetGridObject(object gridObject)
	{
		this.gridObject = gridObject;
	}

	protected virtual void Update()
	{
		gridPositionText.text = gridObject.ToString();
	}
}
