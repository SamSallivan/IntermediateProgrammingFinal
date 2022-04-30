using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	public Transform tJoint;

	public int liftCheckCount;

	public float blink;

	public float liftCheckTimer;

	public float liftTimer;

	public float liftCooldown;

	public float zPos;

	public float yPos;

	public Transform tLifted;

	public Rigidbody rb;

	public Collider clldrLifted;

	public Vector3 dir;

	public Vector3 offset;

	public RaycastHit hit;

	public bool isHolding;

	public Collider[] colliders = new Collider[1];

	public Transform t;

	public WeaponController[] weapons;

	public SlapController slapController { get; private set; }

	public PlayerController p;

	public int currentWeapon;

	public Rigidbody rbLifted;

    public GameObject[] weaponDrops;


	private void Awake()
	{
		t = base.transform;
		weapons = GetComponentsInChildren<WeaponController>(true);
		slapController = GetComponentInChildren<SlapController>();
		p = GetComponentInParent<PlayerController>();
		currentWeapon = -1;
		Deactivate();
	}

	public void DropLiftedObject()
	{
		if ((bool)rbLifted)
		{
			p.bob.Sway(new Vector4(-5f, 0f, 5f, 4f));
			rbLifted.isKinematic = false;
			rbLifted.gameObject.layer = 14;
			rbLifted.AddForce(p.tHead.forward * 5f, ForceMode.Impulse);
			rbLifted.AddTorque(rbLifted.transform.right, ForceMode.Impulse);
			rbLifted = null;
			tLifted = null;
			liftTimer = 0f;
			liftCooldown = 0.25f;
			//Game.soundsManager.PlayClip(objectThrow);
		}
	}

	public void PickWeapon(int index)
	{
		if ((bool)rbLifted)
		{
			DropLiftedObject();
		}
		if (currentWeapon > -1)
		{
			//QuickPool.instance.Get(weapons[currentWeapon].name, p.tHead.position + p.tHead.forward).rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
		}
		currentWeapon = index;
		Refresh();
		//Game.soundsManager.PlayClip(pick);
	}

	public void Drop(Vector3 dir)
	{
		if (currentWeapon > -1)
		{
			Weapon drop = GameObject.Instantiate(weaponDrops[currentWeapon], p.tHead.position, Quaternion.LookRotation(p.tHead.right)).GetComponent<Weapon>();
            drop.Drop(dir * 8f, -90f);
            //((PooledWeapon)QuickPool.instance.Get(weapons[currentWeapon].name, p.tHead.position, Quaternion.LookRotation(p.tHead.right))).Drop(dir * 8f, -90f);
		}
		Pick(-1);
	}

	private void LateUpdate()
	{
		if ((bool)tLifted)
		{
			tLifted.SetPositionAndRotation(tJoint.position + tJoint.TransformDirection(offset), Quaternion.LookRotation(p.tHead.forward));
		}
	}

	private void Update()
	{
		slapController.Tick();
		if (currentWeapon > -1)
		{
			//zPos = Mathf.Lerp(zPos, kickController.isCharging ? (-0.1f) : 0f, Time.deltaTime * 4f);
			zPos = Mathf.Lerp(zPos, 0f, Time.deltaTime * 4f);
			yPos = Mathf.Lerp(yPos, -0.2f, Time.deltaTime * 2f);
			//weapons[currentWeapon].animator.SetBool("Kicking", kickController.isCharging);
			weapons[currentWeapon].animator.SetBool("Sliding", p.slide.slideState != 0);
			weapons[currentWeapon].Tick();
			weapons[currentWeapon].transform.localPosition = new Vector3(0f, yPos, zPos);
		}

		// if ((isHolding && Input.GetKeyUp(p.switchKey)) || Input.GetButtonUp("Dash"))
		// {
		// 	isHolding = false;
		// }

		// if (p.dash.isDashing)
		// {
		// 	return;
		// }

		if ((bool)tLifted)
		{
			if (Input.GetKey(KeyCode.Mouse0) || Input.GetButton("Dash"))
			{
				liftTimer = Mathf.MoveTowards(liftTimer, 1f, Time.deltaTime * 1.75f);
                float temp = Mathf.Lerp(0.25f, 0.65f, liftTimer);
				tJoint.localPosition = new Vector3(temp, temp, temp);
			}
			if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetButtonUp("Dash"))
			{
				p.bob.Sway(new Vector4(10f, 0f, 0f, 2.5f));
				rbLifted.gameObject.layer = 14;
				rbLifted.isKinematic = false;
				rbLifted.AddForce(p.tHead.forward * Mathf.Lerp(20f, 45f, liftTimer), ForceMode.Impulse);
				rbLifted.AddTorque(Vector3.one * (30f * liftTimer), ForceMode.Impulse);
				if (!p.grounder.grounded)
				{
					//rbLifted.GetComponent<BreakableB>().stylePoint = StylePointTypes.SlamDunk;
				}
				rbLifted = null;
				tLifted = null;
				liftTimer = 0f;
				liftCooldown = 0.25f;
				//Game.soundsManager.PlayClip(objectThrow);
			}
			// if (Input.GetKeyDown(p.altKey))
			// {
			// 	DropLiftedObject();
			// }
			// if (((liftCooldown <= 0f && Input.GetKeyDown(p.switchKey)) || Input.GetButtonDown("Dash")) && p.dash.Dash())
			// {
			// 	DropLiftedObject();
			// }
		}
		else if (liftCooldown <= 0f)
		{
			// if (Input.GetKeyDown(p.switchKey) || Input.GetButtonDown("Dash"))
			// {
			// 	if (!p.dash.Dash() && !Lift())
			// 	{
			// 		liftCheckTimer = 0.1f;
			// 		liftCheckCount = 0;
			// 		isHolding = true;
			// 	}
			// 	else
			// 	{
			// 		isHolding = false;
			// 	}
			// }
			// if (isHolding && liftCheckCount < 4 && (Input.GetKey(p.switchKey) || Input.GetButton("Dash")))
			// {
			// 	if (liftCheckTimer <= 0f)
			// 	{
			// 		if (!p.dash.Dash() && !Lift())
			// 		{
			// 			liftCheckTimer = 0.1f;
			// 			liftCheckCount++;
			// 		}
			// 		else
			// 		{
			// 			isHolding = false;
			// 		}
			// 	}
			// 	else
			// 	{
			// 		liftCheckTimer -= Time.deltaTime;
			// 	}
			// }
		}
		else
		{
			liftCooldown -= Time.deltaTime;
		}
	}

	private bool Lift()
	{
		Vector3 vector = p.tHead.forward;
		for (int i = 0; i < 2; i++)
		{
			Physics.Raycast(p.tHead.position, vector, out hit, 3f, 16384);
			if (hit.distance != 0f)
			{
				break;
			}
			vector = Quaternion.AngleAxis(30f, PlayerController.instance.tHead.right) * vector;
			Debug.DrawRay(p.tHead.position, vector, Color.red, 2f);
		}
		if (hit.distance != 0f)
		{
			if (!hit.rigidbody.isKinematic)
			{
				Drop(p.tHead.forward);
				//QuickEffectsPool.Get("Poof", hit.rigidbody.worldCenterOfMass).Play();
				p.bob.Sway(new Vector4(-5f, 0f, 0f, 2f));
				hit.collider.gameObject.layer = 11;
				rbLifted = hit.rigidbody;
				rbLifted.isKinematic = true;
				tLifted = rbLifted.transform;
				offset = tLifted.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
				offset.x = 0f;
				offset.z *= -1f;
				liftTimer = 0f;
				liftCheckCount = 4;
				tJoint.localPosition = new Vector3(0.25f, 0.25f, 0.25f);
				//Game.soundsManager.PlayClip(liftSounds);
				return true;
			}
			return false;
		}
		return false;
	}

	public float KickingOrHolding()
	{
		if ((bool)tLifted)
		{
			return rbLifted.mass * 0.8f * ((liftTimer == 1f) ? 0.2f : 1f);
		}
		// if (kickController.isCharging)
		// {
		// 	return p.grounder.grounded ? 3 : 0;
		// }
		return 0f;
	}

	public float Holding()
	{
		if (currentWeapon <= -1)
		{
			return liftTimer;
		}
		return weapons[currentWeapon].holding;
	}

	public bool IsAttacking()
	{
		if (currentWeapon >= 0)
		{
			return weapons[currentWeapon].attackState == 1;
		}
		return tLifted;
	}

	public bool IsBlocking()
	{
		if (currentWeapon <= -1)
		{
			return false;
		}
		return weapons[currentWeapon].isBlocking;
	}

	public bool Pick(int index)
	{
		if (currentWeapon != index)
		{
			currentWeapon = index;
			Refresh();
			return true;
		}
		return false;
	}

	public void Damage()
	{
		weapons[currentWeapon].DamageReaction();
	}

	public void Refresh()
	{
		for (int i = 0; i < weapons.Length; i++)
		{
			if (i == currentWeapon)
			{
				weapons[i].gameObject.SetActive(true);
			}
			else
			{
				weapons[i].gameObject.SetActive(false);
			}
		}
	}

	public void Deactivate()
	{
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].gameObject.SetActive(false);
		}
	}
}

