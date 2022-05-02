using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlapController : MonoBehaviour
{
	public Damage damage = new Damage();

	public AudioClip chargeSound;

	public AudioClip kickSound;

	public AudioClip hitSound;

	public AudioClip sfxMidAirHit;

	public AudioClip sfxWallKickJump;

	public Animator animator;

	public PlayerController player;

	public WeaponManager manager;
	
	public LayerMask slapMask = 58368;

	public float minDistance;

	public float distance;

	public RaycastHit hit;

	public Collider targetCollider;

	public Collider[] colliders = new Collider[3];

	public bool isCharging;

	public float timer;

	public GameObject slapVFX; 

	private void Awake()
	{
		animator = GetComponent<Animator>();
		player = GetComponentInParent<PlayerController>();
		manager = GetComponentInParent<WeaponManager>();
		Grounder grounder = player.grounder;
	}

	private void OnDestroy()
	{
		Grounder grounder = player.grounder;
		grounder.OnUnground -= Ungrounded;
		grounder.OnGround -= Grounded;
	}

	private void Grounded()
	{
		if (isCharging)
		{
			animator.SetTrigger("Charge");
		}
	}

	private void Ungrounded()
	{
		if (isCharging)
		{
			animator.SetTrigger("Charge");
		}
	}

	private void OnEnable()
	{
		if (isCharging)
		{
			isCharging = false;
		}
	}

	public void ChargeSway()
	{
		player.bob.Sway(new Vector4(-2.5f, 0f, 0f, 2f));
	}
	public void StrikeSway()
	{
		player.bob.Sway(new Vector4(0f, -10f, 0f, 3f));
	}

	private void OnDrawGizmos()
	{
		if ((bool)player)
		{
			Gizmos.matrix = player.tHead.localToWorldMatrix;
			Gizmos.DrawWireCube(new Vector3(0f, 0f, 1.25f), new Vector3(1.4f, 2.5f, 2.5f));
		}
	}

	public void Strike()
	{
		targetCollider = null;
		minDistance = float.PositiveInfinity;
		if (player.grounder.grounded && player.slide.slideState == 0)
		{
			Physics.OverlapBoxNonAlloc(player.tHead.position + player.tHead.forward * 1.25f, new Vector3(0.7f, 1.25f, 1.25f), colliders, player.tHead.rotation, slapMask);
		}
		else
		{
			Physics.OverlapCapsuleNonAlloc(player.tHead.position, player.tHead.position + player.tHead.forward * 3.5f, 1f, colliders, slapMask);
		}
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i] != null)
			{
				distance = Vector3.Distance(player.tHead.position, colliders[i].ClosestPoint(player.tHead.position));
				if (distance < minDistance)
				{
					minDistance = distance;
					targetCollider = colliders[i];
				}
				colliders[i] = null;
			}
		}
		if ((bool)targetCollider && !Physics.Linecast(player.tHead.position, targetCollider.ClosestPoint(player.tHead.position), 1))
		{
			Vector3 vector = targetCollider.ClosestPoint(player.tHead.position);
			if (!player.grounder.grounded)
			{
				// if (player.tHead.forward.y > 0f - minKickJumpDot)
				// {
					player.rb.position = Vector3.Lerp(player.rb.position, vector, 0.75f);
					player.rb.velocity = Vector3.zero;
				// }
				//player.rb.AddForce((Vector3.up - player.tHead.forward/ 4f).normalized * 20f, ForceMode.Impulse);
				player.rb.AddForce((Vector3.up - player.tHead.forward / 4f).normalized * 5, ForceMode.Impulse);
				player.airControlBlockTimer = 0.25f;
				if (targetCollider.gameObject.layer == 10 && targetCollider.attachedRigidbody.isKinematic)
				{
					damage.dir = player.tHead.forward/2;
				}
				else
				{
					damage.dir = player.tHead.forward;
				}
				player.airControl = 1;

				if (targetCollider.gameObject.layer != 13)
				{
					damage.amount = 25;
					damage.dir = Vector3.zero;
					targetCollider.GetComponent<Damagable>().Damage(damage);
								
					// player.slamVFX.transform.position = targetCollider.transform.position;
					// player.slamVFX.transform.rotation = Quaternion.LookRotation(targetCollider.transform.forward);
					// player.slamVFX.GetComponent<ParticleSystem>().Play();
				}
				else
				{
					timer = 0f;
					targetCollider.GetComponent<Slappable>().Slap(player.tHead.forward);
				}
				if (targetCollider.gameObject.layer == 10)
				{
					//StylePointsCounter.instance.AddStylePoint(StylePointTypes.AirKick);
				}
				player.grounder.Unground();
				//Game.soundsManager.PlayClip(sfxMidAirHit);
			}
			else
			{
				Vector3 forward = player.tHead.forward;
				targetCollider.GetComponent<Slappable>().Slap(forward);
				damage.amount = 25;
				damage.dir = Vector3.zero;
				targetCollider.GetComponent<Damagable>().Damage(damage);
				// if (player.tHead.forward.y > 0f - minKickJumpDot && player.slide.slideState != 0)
				// {
				if (player.slide.slideState != 0)
				{
					player.rb.position = Vector3.Lerp(player.rb.position, vector, 0.75f);
					player.rb.velocity = Vector3.zero;
				}
								
				// player.slamVFX.transform.position = targetCollider.transform.position;
				// player.slamVFX.transform.rotation = Quaternion.LookRotation(targetCollider.transform.forward);
				// player.slamVFX.GetComponent<ParticleSystem>().Play();
			}
			// trail.transform.SetPositionAndRotation(vector, Quaternion.LookRotation(-player.tHead.forward));
			// trail.Play();
			// trailParticle.Emit(10);
			player.bob.Sway(new Vector4(10f, 0f, 0f, 5f));
			//CameraController.shake.Shake(2);
			if (player.slide.slideState > 0 && targetCollider.gameObject.layer == 10)
			{
				//StylePointsCounter.instance.AddStylePoint(StylePointTypes.SlideKick);
			}
		}
		else
		{
			if (!Physics.Raycast(player.tHead.position, player.tHead.forward, out hit, 2.5f, 1))
			{
				return;
			}
			if (player.slide.slideState > 0 && hit.normal.y > 0.5f)
			{
				player.Jump(1.6f);
			}
			else
			{
				player.rb.AddForce(-player.tHead.forward * Mathf.Lerp(8f, 4f, Mathf.Abs(player.tHead.forward.y)), ForceMode.Impulse);
				player.bob.Sway(new Vector4(5f, 0f, 0f, 5f));
				//QuickEffectsPool.Get("Poof", player.tHead.position + player.tHead.forward).Play();
			}
		}
	}

	public void Tick()
	{
		if (manager.currentWeapon == -1)
		{
			if (timer != 0f)
			{
				if (isCharging)
				{
					animator.ResetTrigger("Release");
					animator.SetTrigger("Cancel");
					isCharging = false;
				}
				timer = Mathf.MoveTowards(timer, 0f, Time.deltaTime * 2f);
			}
			else if (!isCharging)
			{
				if (!manager.IsAttacking() && Input.GetKeyDown(KeyCode.Mouse0))
				{
					animator.SetTrigger("Charge");
					isCharging = true;
					if (player.slide.slideState > 0)
					{
					player.slide.Extend(1f);
					// 	player.slide.DeactivateTrigger();
					TimeManager.instance.SlowMotion(0.1f, 1.5f, 0.25f);
					}
					//Game.soundsManager.PlayClip(chargeSound);
				}
			}
			else if (!Input.GetKey(KeyCode.Mouse0))
			{
				animator.SetTrigger("Release");
				isCharging = false;
				timer = 1f;
				manager.zPos = -1f;
				slapVFX.GetComponent<ParticleSystem>().Play();
				//player.bob.Sway(new Vector4(0f, -20f, 0f, 3f));
				TimeManager.instance.StopSlowmo();
				// Game.soundsManager.PlayClip(kickSound);
			}
		}
		else{
			animator.ResetTrigger("Release");
			animator.SetTrigger("Cancel");

		}
	}
}
