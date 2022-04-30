using System;
using UnityEngine;

public abstract class WeaponController : MonoBehaviour
{
	// public Weapon weapon;

	public PlayerController player;

	public WeaponManager manager;

	public bool midAirAction;

	public float holding;

	// protected DamageInfo dmgInfo = new DamageInfo();

	public Collider[] colliders = new Collider[3];

	public int attackIndex;

	public int attackState;

	public bool isBlocking;

	public Animator animator;

	public virtual void DamageReaction()
	{
	}

	protected virtual void Awake()
	{
		attackIndex = -1;
		attackState = 0;
		animator = GetComponent<Animator>();
		player = GetComponentInParent<PlayerController>();
		manager = GetComponentInParent<WeaponManager>();
	}

// 	public void Parry()
// 	{
// 		if ((bool)CrowdControl.lastAttacked)
// 		{
// 			CrowdControl.lastAttacked.Kick(player.tHead.forward);
// 			StylePointsCounter.instance.AddStylePoint(StylePointTypes.PerfectParry);
// 		}
// 	}

// 	public void Stun(Vector3 slashBoxSize)
// 	{
// 		bool flag = false;
// 		Physics.OverlapBoxNonAlloc(player.tHead.position + player.tHead.forward * slashBoxSize.z / 2f, slashBoxSize, colliders, player.tHead.rotation, 17408);
// 		for (int i = 0; i < colliders.Length; i++)
// 		{
// 			if (colliders[i] != null)
// 			{
// 				dmgInfo.dir = (player.tHead.forward.With(null, 0f) + Vector3.up).normalized;
// 				colliders[i].GetComponent<IDamageable<DamageInfo>>().Damage(dmgInfo);
// 				colliders[i] = null;
// 				flag = true;
// 				StylePointsCounter.instance.AddStylePoint(StylePointTypes.GroundPound);
// 			}
// 		}
// 	}

	// public bool Slash2(Vector3 slashBoxSize)
	// {
	// 	bool result = false;
	// 	Physics.OverlapBoxNonAlloc(player.tHead.position + player.tHead.forward * slashBoxSize.z / 2f, slashBoxSize, colliders, player.tHead.rotation, 17408);
	// 	for (int i = 0; i < colliders.Length; i++)
	// 	{
	// 		if (colliders[i] != null)
	// 		{
	// 			colliders[i].GetComponent<Damagable>().Damage(dmgInfo);
	// 			colliders[i] = null;
	// 			//dmgInfo.amount *= 0.75f;
	// 			result = true;
	// 		}
	// 	}
	// 	return result;
	// }

// 	public bool Slash3(Vector3 slashBoxSize)
// 	{
// 		bool result = false;
// 		Physics.OverlapBoxNonAlloc(player.tHead.position + player.tHead.forward * slashBoxSize.z / 2f, slashBoxSize, colliders, player.tHead.rotation, 17409);
// 		for (int i = 0; i < colliders.Length; i++)
// 		{
// 			if (!(colliders[i] != null))
// 			{
// 				continue;
// 			}
// 			if (colliders[i].gameObject.layer != 0)
// 			{
// 				colliders[i].GetComponent<IDamageable<DamageInfo>>().Damage(dmgInfo);
// 				dmgInfo.amount *= 0.9f;
// 				if (colliders[i].gameObject.layer == 14)
// 				{
// 					result = true;
// 				}
// 			}
// 			else
// 			{
// 				result = true;
// 			}
// 			colliders[i] = null;
// 		}
// 		return result;
// 	}

 	public abstract void Tick();

// 	protected virtual void CancelAttack()
// 	{
// 		attackIndex = -1;
// 		animator.SetInteger("Attack Index", attackIndex);
// 		animator.SetTrigger("Cancel");
// 		attackState = 0;
// 	}

// 	private void OnDisable()
// 	{
// 		holding = 0f;
// 	}

// 	private void OnDestroy()
// 	{
// 		PlayerHead.OnGameQuickReset = (Action)Delegate.Remove(PlayerHead.OnGameQuickReset, new Action(Reset));
// 	}

// 	protected virtual void Reset()
// 	{
// 		attackIndex = -1;
// 		attackState = 0;
// 	}
}
