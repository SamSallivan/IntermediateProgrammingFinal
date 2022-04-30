using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollCollider : MonoBehaviour, Damagable, Slappable
{
	public EnemyMove enemy { get; private set; }

	private void Awake()
	{
		enemy = GetComponentInParent<EnemyMove>();
	}

	public void Slap(Vector3 dir)
	{
		if(enemy.doll){
			enemy.Slap(dir);
		}
	}

	public void Damage(Damage damage)
	{
		if(enemy.doll)
			enemy.Damage(damage);
	}


}
