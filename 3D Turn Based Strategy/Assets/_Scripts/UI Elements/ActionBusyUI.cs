using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    void Start()
    {
          UnitActionSystem.Instance.OnSetActionBusy += SetBusy;
          UnitActionSystem.Instance.OnUnsetActionBusy += UnsetBusy;

          gameObject.SetActive(false);
    }

     private void SetBusy(object sender, EventArgs e)
     {
          gameObject.SetActive(true);
     }

     private void UnsetBusy(object sender, EventArgs e)
	{
          gameObject.SetActive(false);
	}
}
