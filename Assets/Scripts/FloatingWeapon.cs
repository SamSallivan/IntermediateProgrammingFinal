using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingWeapon : Weapon
{
	public Transform tMesh;

	protected override void Awake()
	{
		base.Awake();
		tMesh = base.t.GetChild(0).transform;
	}

	private void Update()
	{
		if ((bool)tMesh)
		{
			tMesh.localPosition = new Vector3(0f, Mathf.Sin(Time.timeSinceLevelLoad) * 0.025f, 0f);
			tMesh.Rotate(Vector3.up * (360f * Time.deltaTime));
		}
	}

	public override void Slap(Vector3 dir)
	{

	}

	public override void Interact(WeaponManager manager)
	{
		base.Interact(manager);
		//QuickEffectsPool.Get("WeaponPick", base.t.position + PlayerController.instance.tHead.forward * 2f, PlayerController.instance.tHead.rotation).Play();
	}
}
