using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{

	public float amount;

	public Vector3 dir;

	public Damage(float amount = 0f, Vector3 dir = default(Vector3))
	{
		this.amount = amount;
		this.dir = dir;
	}

}
