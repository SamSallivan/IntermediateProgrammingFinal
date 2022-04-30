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

	//public float minKickJumpDot;

	public float minDistance;

	public float distance;

	public RaycastHit hit;

	public Collider targetCollider;

	public Collider[] colliders = new Collider[3];

	//private TrailScript trail;

	//private ParticleSystem trailParticle;

	public bool isCharging;

	public float timer;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		player = GetComponentInParent<PlayerController>();
		manager = GetComponentInParent<WeaponManager>();
		// trail = GetComponentInChildren<TrailScript>();
		// trail.transform.SetParent(null);
		// trailParticle = trail.GetComponent<ParticleSystem>();
		Grounder grounder = player.grounder;
		//grounder.OnUnground += Ungrounded;
		//grounder.OnGround += Grounded;
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
			//Game.soundsManager.PlayClip(chargeSound);
		}
	}

	private void Ungrounded()
	{
		if (isCharging)
		{
			animator.SetTrigger("Charge");
			//Game.soundsManager.PlayClip(chargeSound);
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
		player.bob.Sway(new Vector4(-5f, 0f, 0f, 2f));
	}
	public void StrikeSway()
	{
		player.bob.Sway(new Vector4(0f, -20f, 0f, 3f));
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
				player.rb.AddForce((Vector3.up - player.tHead.forward.With(null, 0f).normalized / 4f).normalized * 20f, ForceMode.Impulse);
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
					//damage.amount = 50;
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
				//damage.amount = 50;
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
			player.airControl = 1;
			player.airControlBlockTimer = 0.25f;
			if (Mathf.Abs(hit.normal.y) < 0.5f && !player.grounder.grounded)
			{
				// if ((bool)player.grippableObject)
				// {
				// 	player.grippableObject.Drop();
				// }

				player.rb.velocity = Vector3.zero;
                Vector3 temp = new Vector3(player.tHead.forward.x, (0f - hit.normal.y) * 2f, player.tHead.forward.z);
                
				//player.rb.AddForce((Vector3.up - temp).normalized * (22.5f * (1f + Mathf.Abs(hit.normal.y))), ForceMode.Impulse);
				player.rb.AddForce((Vector3.up - player.tHead.forward.With(null, (0f - hit.normal.y) * 2f).normalized).normalized * (22.5f * (1f + Mathf.Abs(hit.normal.y))), ForceMode.Impulse);
				
				player.bob.Sway(new Vector4(0f, 0f, 10f, 5f));
				// QuickEffectsPool.Get("Poof", hit.point, Quaternion.LookRotation(hit.normal)).Play();
				// Game.soundsManager.PlayClipAtPosition(sfxWallKickJump, 1f, player.t.position);
			}
			else if (player.slide.slideState > 0 && hit.normal.y > 0.5f)
			{
				player.Jump(1.1f);
				// QuickEffectsPool.Get("Poof", player.t.position).Play();
				// Game.soundsManager.PlayClipAtPosition(sfxWallKickJump, 1f, player.t.position);
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
		if (manager.currentWeapon!=-1 || timer != 0f)
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
				// 	player.slide.Extend();
				// 	player.slide.DeactivateTrigger();
				// 	Game.timeManager.SlowMotion(0.1f, 1.5f, 0.1f);
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
			//player.bob.Sway(new Vector4(0f, -20f, 0f, 3f));
                // Game.timeManager.StopSlowmo();
                // Game.soundsManager.PlayClip(kickSound);
		}
	}
}
