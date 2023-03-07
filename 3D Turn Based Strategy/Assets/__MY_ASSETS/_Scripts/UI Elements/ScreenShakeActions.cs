using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
	private void Start()
	{
		ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
		Grenade.OnAnyGrenadeEvent += Grenade_OnAnyGrenadeEvent;
	}

	private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
	{
		ScreenShake.Instance.Shake();
	}

	private void Grenade_OnAnyGrenadeEvent(object sender, EventArgs e)
	{
		ScreenShake.Instance.Shake(5f);
	}
}
