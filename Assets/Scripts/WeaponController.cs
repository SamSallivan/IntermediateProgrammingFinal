using System;
using UnityEngine;

public abstract class WeaponController : MonoBehaviour
{
	public PlayerController player;

	public WeaponManager manager;

	public float holding;

	public Collider[] colliders = new Collider[3];

	public int attackIndex;

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

	public virtual void Block()
	{
		
	}

 	public virtual void Tick(){

	}
}
