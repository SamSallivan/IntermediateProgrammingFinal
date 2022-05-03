using System;
using UnityEngine;

//Base Class for all weapons.
//Currently there is only sword.
//Saved for future development.
public abstract class WeaponController : MonoBehaviour
{
	public PlayerController player;

	public WeaponManager manager;

	//Charge timer.

	public float holding;

	//Allows to attack up to 3 targets.
	public Collider[] colliders = new Collider[3];

	//Index for attack types.
	public int attackIndex;

	//State of the player in terms of attack.
	public int attackState;

	public bool isBlocking;

	public Animator animator;

	protected virtual void Awake()
	{
		attackIndex = -1;
		attackState = 0;
		animator = GetComponent<Animator>();
		player = GetComponentInParent<PlayerController>();
		manager = GetComponentInParent<WeaponManager>();
	}

	//each weapon will have a block funtion.
	public virtual void Block()
	{
		
	}

	//each weapon will have a input receiver funtion.
 	public virtual void WeaponInputeUpdate(){

	}
}
