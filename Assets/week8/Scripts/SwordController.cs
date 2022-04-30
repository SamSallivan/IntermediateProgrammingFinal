using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : WeaponController
{
	//public TrailScript trail;

	protected Damage damage = new Damage();

	public GameObject objWeapon;

	public Color colorA;

	public Color colorB;

	public AudioClip clip;

	public AudioClip sfxSwitchA;

	public AudioClip sfxSwitchB;

	public AudioClip sfxSlashCharge;

	public AudioClip sfxSlamSlash;

	public AudioClip sfxBlock;

	public AnimationCurve slashDamageCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AnimationCurve slamSlashDamageCurve = AnimationCurve.Linear(0f, 40f, 1f, 75f);

	public bool rightSlash;

	public float rightSlashTimer;

	public float cooldown;

	private void OnEnable()
	{
		//trail.Reset();
		cooldown = 0f;
		base.attackState = 0;
		if (!objWeapon.activeInHierarchy)
		{
			objWeapon.SetActive(true);
		}
		rightSlashTimer = 0f;
		rightSlash = false;
		player.bob.Sway(new Vector4(0f, 0f, -5f, 2f));
	}

	protected override void Awake()
	{
		base.Awake();
		Grounder grounder = player.grounder;
		grounder.OnGround += Ground;
	}

	private void OnDestroy()
	{
		Grounder grounder = player.grounder;
		grounder.OnGround -= Ground;
		//grounder.OnGrounded = (Action)Delegate.Remove(grounder.OnGrounded, new Action(Grounded));
	}

	public void Charge()
	{
		switch (base.attackIndex)
		{
		case 0:
		case 1:
			//Game.soundsManager.PlayClip(sfxSlashCharge);
			player.bob.Sway(new Vector4(0f, 0f, rightSlash ? 5 : (-5), 2f));
			break;
		case 2:
			//Game.soundsManager.PlayClip(sfxSlamSlash);
			player.bob.Sway(new Vector4(-6f, 0f, 0f, 2f));
			break;
		}
	}

	public void Sway()
	{
		switch (base.attackIndex)
		{
		case 0:
		case 1:
			//Game.soundsManager.PlayClip(clip);
			player.bob.Sway(new Vector4(0f, Mathf.Lerp(10f, 30f, holding) * (float)((base.attackIndex == 0) ? 1 : (-1)), 0f, 5f));
			//trail.t.SetPositionAndRotation(player.tHead.position - player.tHead.up * 0.2f, Quaternion.LookRotation(player.tHead.forward, manager.t.up * ((base.attackIndex != 0) ? 1 : (-1))));
			//trail.gameObject.SetActive(true);
			//trail.SetColor(Color.Lerp(colorA, colorB, holding));
			break;
		case 2:
			//Game.soundsManager.PlayClip(clip);
			player.bob.Sway(new Vector4(Mathf.Lerp(10f, 30f, holding), 0f, 0f, 5f));
			//trail.t.SetPositionAndRotation(player.tHead.position, Quaternion.LookRotation(player.tHead.forward, -manager.t.right));
			//trail.gameObject.SetActive(true);
			//trail.SetColor(Color.Lerp(colorA, colorB, holding));
			break;
		case 3:
			player.bob.Sway(new Vector4(10f, 0f, 0f, 5f));
			break;
		case 4:
			//Game.soundsManager.PlayClip(sfxBlock);
			player.bob.Sway(new Vector4(0f, 0f, 5f, 4f));
			break;
		}
	}


	public bool Slash2(Vector3 slashBoxSize)
	{
		bool result = false;
		Physics.OverlapBoxNonAlloc(player.tHead.position + player.tHead.forward * slashBoxSize.z / 2f, slashBoxSize, colliders, player.tHead.rotation, 17408);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i] != null)
			{
				colliders[i].GetComponent<Damagable>().Damage(damage);
				colliders[i] = null;
				//dmgInfo.amount *= 0.75f;
				result = true;
							
			}
		}
		//Debug.Log(result);
		return result;
	}

	public void Strike()
	{
		//manager.Blink(0f);
		switch (base.attackIndex)
		{
		case 0:
		case 1:
			damage.dir = ((player.tHead.forward + player.tHead.right * ((base.attackIndex == 0) ? 1 : (-1))) / 2f).normalized;
			damage.amount = slashDamageCurve.Evaluate(holding);
			// damage.type = DamageInfo.DamageType.MidRip;
			// damage.knockdown = false;
			if (Slash2(new Vector3(2f, 0.5f, 2.5f)))
			{
				//QuickEffectsPool.Get("Slash Hit", player.tHead.position + player.tHead.forward * 1.25f, Quaternion.LookRotation(player.tHead.right * ((base.attackIndex == 0) ? 1 : (-1)))).Play(holding);
			}
			else
			{
				Vector3 forward = player.tHead.forward;
				forward = Quaternion.AngleAxis(40f, player.tHead.up) * forward;
				for (int i = 0; i < 3; i++)
				{
					Debug.DrawRay(player.tHead.position, forward * 3f, Color.red, 2f);
					RaycastHit hitInfo;
					if (Physics.Raycast(player.tHead.position, forward, out hitInfo, 2.5f, 1))
					{
						//QuickEffectsPool.Get("Slash Hit B", hitInfo.point, Quaternion.LookRotation(player.tHead.right * ((base.attackIndex == 0) ? 1 : (-1)))).Play(holding);
						break;
					}
					forward = Quaternion.AngleAxis(-40f, player.tHead.up) * forward;
				}
			}
			rightSlash = !rightSlash;
			if (rightSlash)
			{
				rightSlashTimer = 1f;
			}
			break;
		case 2:
		{
			damage.dir = ((player.tHead.forward + player.tHead.up / 2f) / 2f).normalized;
			damage.amount = slamSlashDamageCurve.Evaluate(holding);
			// damage.type = DamageInfo.DamageType.VerticalSlash;
			// damage.knockdown = holding >= 0.35f;
			bool flag = Slash2(new Vector3(0.5f, 2f, 3f));
			// CameraController.shake.Shake((!flag) ? 1 : 2);
			// QuickEffectsPool.Get(flag ? "Slash Hit" : "Slash Hit B", player.t.position + player.tHead.forward * 2f, Quaternion.LookRotation(player.tHead.forward.With(null, 0f))).Play(holding);
			// if (holding == 1f)
			// {
			// 	QuickPool.instance.Get("Floor Chaser", player.t.position - Vector3.up, player.tHead.rotation);
			// 	midAirAction = false;
			// 	player.midairActionPossible = false;
			// }
			 break;
		}
		case 3:
			// if (holding == 1f)
			// {
			// 	Vector3 vector = player.tHead.forward;
			// 	if (player.slide.slideState != 0)
			// 	{
			// 		//vector = CrowdControl.instance.GetClosestDirectionToNormal(player.tHead.position, vector, 7.5f);
			// 	}
			// 	//(QuickPool.instance.Get("Throwed Sword", player.tHead.position, Quaternion.LookRotation(vector)) as ThrowedSword).stylePoint = (player.grounder.grounded ? StylePointTypes.SwordThrow : StylePointTypes.AirSwordThrow);
            //     Weapon drop = GameObject.Instantiate(manager.weaponDrops[manager.currentWeapon], player.tHead.position + player.tHead.forward*5, Quaternion.LookRotation(player.tHead.right)).GetComponent<Weapon>();
            //     drop.Drop(player.tHead.forward * Mathf.Lerp(5f, 15f, holding), -90f);
            // }
			// else
			// {
				//((PooledWeapon)QuickPool.instance.Get("Sword", player.tHead.position, Quaternion.LookRotation(player.tHead.up))).Drop(player.tHead.forward * (5f + 10f * holding), 90f);
                Weapon drop = GameObject.Instantiate(manager.weaponDrops[manager.currentWeapon], player.tHead.position + player.tHead.forward*5, Quaternion.LookRotation(player.tHead.right)).GetComponent<Weapon>();
                float dropDistance = Mathf.Lerp(5f, 15f, holding);
				//Debug.Log(player.tHead.forward * dropDistance);
				drop.Drop(player.tHead.forward * dropDistance, -90f);
            //}
			objWeapon.SetActive(false);
			 break;
		}
		holding = 0f;
		base.attackState = 0;
		base.attackIndex = -1;
		base.animator.SetInteger("Attack Index", base.attackIndex);
	}

	public void Drop()
	{
		manager.Pick(-1);
	}

	public override void DamageReaction()
	{
		base.DamageReaction();
		if (base.attackIndex == 4)
		{
			base.animator.SetTrigger("Damage");
			if (holding < 0.5f)
			{
				//Parry();
			}
			else
			{		
				//Game.timeManager.SlowMotion(0.1f, 0.3f, 0.2f);
				//((PooledWeapon)QuickPool.instance.Get("Sword", player.tHead.position, Quaternion.LookRotation(player.tHead.right))).Drop(player.tHead.forward * 5f, -90f);
				Weapon drop = Instantiate(manager.weaponDrops[manager.currentWeapon], player.tHead.position + player.tHead.forward*5, Quaternion.LookRotation(player.tHead.right)).GetComponent<Weapon>();
				drop.Drop(player.tHead.forward * 5, -90f);
				manager.Pick(-1);		
			}
			//CameraController.shake.Shake(2);
			base.isBlocking = false;
			base.attackIndex = -1;
			base.attackState = 0;
			base.animator.SetInteger("Attack Index", base.attackIndex);
			holding = 0f;
		}
	}

	private void Ground()
	{
		if (base.animator.GetInteger("Attack Index") == 2)
		{
			base.animator.SetTrigger("Release");
			cooldown = 0.1f;
		}
	}

	private void ChargeAttackWithIndex(int i, float newHolding = 0f)
	{
		base.attackIndex = i;
		base.animator.SetInteger("Attack Index", base.attackIndex);
		base.animator.SetTrigger("Charge");
		holding = newHolding;
	}

	private void OldBlock()
	{
		if (base.isBlocking)
		{
			holding = Mathf.MoveTowards(holding, 1f, Time.deltaTime * 2f);
			if (holding == 1f)
			{
				base.isBlocking = false;
				holding = 0f;
			}
		}
		else if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			base.isBlocking = true;
			base.animator.SetTrigger("Block");
			holding = 0f;
		}
	}

	public override void Tick()
	{
		if (cooldown > 0f)
		{
			cooldown -= Time.deltaTime;
			return;
		}
		base.isBlocking = base.attackIndex == 4 && base.attackState != 0;
		switch (base.attackState)
		{
		case 0:
			if (!objWeapon.activeInHierarchy)
			{
				break;
			}
            
			if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse1))
			{
                player.bob.Sway(new Vector4(0f, 0f, 5f, 3f));
				ChargeAttackWithIndex(4);
				base.attackState = 1;
			}
			else if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				if (player.grounder.grounded)
				{
					if (player.slide.slideState != 0)
					{
						ChargeAttackWithIndex(3);
						player.bob.Sway(new Vector4(0f, 0f, 5f, 3f));
					}
					else
					{
						ChargeAttackWithIndex(rightSlash ? 1 : 0);
					}
				}
				else
				{
					ChargeAttackWithIndex(2);
					if (player.airControl != 0f)
					{
						midAirAction = true;
					}
				}
				base.attackState = 1;
			}
			else if (Input.GetKeyDown(KeyCode.Mouse1))
			{
				ChargeAttackWithIndex(3);
				base.attackState = 1;
			}
			else if (rightSlashTimer > 0f)
			{
				rightSlashTimer -= Time.deltaTime;
			}
			else if (rightSlash)
			{
				rightSlash = false;
			}
			break;
		case 1:
			switch (base.attackIndex)
			{

            //slashing
			case 0:
			case 1:
				if (Input.GetKeyDown(KeyCode.Mouse1))
				{
					ChargeAttackWithIndex(4);
					player.bob.Sway(new Vector4(0f, 0f, 5f, 3f));
					//Game.soundsManager.PlayClip(sfxSwitchA);
				}
				if (Input.GetKeyDown(KeyCode.Space))
				{
					//Game.soundsManager.PlayClip(sfxSwitchB);
					ChargeAttackWithIndex(2);
				}
				if (!Input.GetKey(KeyCode.Mouse0) && holding > 0.25f)
				{
					base.animator.SetTrigger("Release");
					base.attackState++;
				}
				holding = Mathf.MoveTowards(holding, 1f, Time.deltaTime * 1.5f);
				break;

            //aiming
			case 3:

				if (Input.GetKeyUp(KeyCode.Mouse1))
				{
					//ChargeAttackWithIndex(0, holding);
                    base.attackState = 0;
					base.attackIndex = -1;
					base.animator.SetInteger("Attack Index", base.attackIndex);
					base.animator.SetTrigger("Cancel");
					//Game.soundsManager.PlayClip(sfxSwitchA);
				}

				//Input.GetKeyDown(KeyCode.Space);

				// if (Input.GetKeyDown(KeyCode.Mouse0) && holding < 0.75f)
				// {
				// 	ChargeAttackWithIndex(4);
				// 	base.animator.SetTrigger("Cancel");
				// 	player.bob.Sway(new Vector4(0f, 0f, 5f, 3f));
				// }
				// else 
				if (Input.GetKeyDown(KeyCode.Mouse0)) // && holding >= 0.75f)
				{
					base.animator.SetTrigger("Release");
					base.attackState++;
				}
				holding = Mathf.MoveTowards(holding, 1f, Time.deltaTime * 2.5f);
				break;

            //air attacking
			case 2:
				if (Input.GetKey(KeyCode.Mouse0))
				{
					player.extraUpForce = true;
				}
				if (Input.GetKeyDown(KeyCode.Mouse1))
				{
					ChargeAttackWithIndex(3, holding);
					if (base.attackIndex == 3)
					{
						player.bob.Sway(new Vector4(0f, 0f, 5f, 3f));
					}
					//Game.soundsManager.PlayClip(sfxSwitchA);
				}
				if (Input.GetKeyUp(KeyCode.Mouse0))
				{
					player.rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
					base.animator.SetTrigger("Cancel");
				}
				holding = Mathf.MoveTowards(holding, 1f, Time.deltaTime);
				break;

			case 4:
				holding = Mathf.MoveTowards(holding, 1f, Time.deltaTime * 2.5f);
				if (!Input.GetKey(KeyCode.Mouse1) && holding > 0.5f)
				{
					base.animator.SetTrigger("Cancel");
					base.attackIndex = -1;
					base.animator.SetInteger("Attack Index", base.attackIndex);
					holding = 0f;
					base.attackState = 0;
				}
				break;
			}
			break;
		case 2:
			break;
		}
	}
}
