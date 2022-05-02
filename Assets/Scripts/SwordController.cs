using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : WeaponController
{
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

	public GameObject slapVFX; 

	private void OnEnable()
	{
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
	}

	public void Charge()
	{
		switch (base.attackIndex)
		{
		case 0:
		case 1:
			player.bob.Sway(new Vector4(0f, 0f, rightSlash ? 5 : (-5), 2f));
			break;
		case 2:
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
			player.bob.Sway(new Vector4(0f, Mathf.Lerp(10f, 30f, holding) * (float)((base.attackIndex == 0) ? 1 : (-1)), 0f, 5f));
			break;
		case 2:
			player.bob.Sway(new Vector4(Mathf.Lerp(10f, 30f, holding), 0f, 0f, 5f));
			break;
		case 3:
			player.bob.Sway(new Vector4(10f, 0f, 0f, 5f));
			break;
		case 4:
			player.bob.Sway(new Vector4(0f, 0f, 5f, 4f));
			break;
		}
	}


	public bool SlashCheck(Vector3 slashBoxSize)
	{
		bool result = false;
		Physics.OverlapBoxNonAlloc(player.tHead.position + player.tHead.forward * slashBoxSize.z / 2f, slashBoxSize, colliders, player.tHead.rotation, 17408);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i] != null)
			{
				colliders[i].GetComponent<Damagable>().Damage(damage);
				colliders[i] = null;
				result = true;
							
			}
		}
		return result;
	}

	public void Strike()
	{
		switch (base.attackIndex)
		{
		case 0:
		case 1:
			damage.dir = ((player.tHead.forward + player.tHead.right * ((base.attackIndex == 0) ? 1 : (-1))) / 2f).normalized;
			damage.amount = slashDamageCurve.Evaluate(holding);
			if(!SlashCheck(new Vector3(2f, 0.5f, 2.5f)))
			{
				Vector3 forward = player.tHead.forward;
				forward = Quaternion.AngleAxis(40f, player.tHead.up) * forward;
				for (int i = 0; i < 3; i++)
				{
					Debug.DrawRay(player.tHead.position, forward * 3f, Color.red, 2f);
					RaycastHit hitInfo;
					if (Physics.Raycast(player.tHead.position, forward, out hitInfo, 2.5f, 1))
					{
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
			bool flag = SlashCheck(new Vector3(0.5f, 2f, 3f));
			 break;
		}
		case 3:
			 Weapon drop = GameObject.Instantiate(manager.weaponDrops[manager.currentWeapon], player.tHead.position - player.tHead.forward*1f, Quaternion.LookRotation(player.tHead.right)).GetComponent<Weapon>();
			float dropDistance = Mathf.Lerp(5f, 15f, holding);
			drop.Drop(player.tHead.forward * dropDistance, -90f);
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

	public override void Block()
	{
		if (base.attackIndex == 4)
		{
			base.animator.SetTrigger("Damage");
			if (holding >= 0.5f)
			{	
				TimeManager.instance.SlowMotion(0.1f, 0.3f, 0.2f);
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
					ChargeAttackWithIndex(rightSlash ? 1 : 0);
				}
				else
				{
					ChargeAttackWithIndex(2);
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
                    base.attackState = 0;
					base.attackIndex = -1;
					base.animator.SetInteger("Attack Index", base.attackIndex);
					base.animator.SetTrigger("Cancel");
				}

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
