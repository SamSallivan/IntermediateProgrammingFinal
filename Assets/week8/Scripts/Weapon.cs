using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public Weapon associatedScriptableObject;

	public int weaponIndex;

	public bool keepActiveOnReset;

	public ParticleSystem particle;

	[Header("SFXs")]
	public AudioClip sfxGroundedKick;

	public AudioClip sfxFailedPick;

	public FixedJoint joint;

	//private Body jointedBody;

	public Rigidbody jointedRigidBody;

	public Transform t;

	public Rigidbody rb;

	protected AudioSource source;

	public virtual void Kick(Vector3 dir)
	{
		if (rb.isKinematic)
		{
			//QuickEffectsPool.Get("Poof", base.t.position, base.t.rotation).Play();
			if ((bool)source)
			{
				//source.PlayClip(sfxGroundedKick);
			}
		}
	}

	protected virtual void Awake()
	{
		t = base.transform;
		rb = GetComponentInChildren<Rigidbody>();
	}

	private void LateUpdate()
	{
		if ((bool)particle)
		{
			Vector3 eulerAngles = t.eulerAngles;
			ParticleSystem.MainModule main = particle.main;
			main.startRotationX = eulerAngles.x * ((float)Mathf.PI / 180f);
			main.startRotationY = eulerAngles.y * ((float)Mathf.PI / 180f);
			main.startRotationZ = eulerAngles.z * ((float)Mathf.PI / 180f);
		}
	}

	private void Reset()
	{
		if ((bool)joint)
		{
			UnityEngine.Object.Destroy(GetComponent<FixedJoint>());
			//jointedBody = null;
			jointedRigidBody = null;
		}
		if (keepActiveOnReset)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				base.gameObject.SetActive(true);
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void CheckJoint(GameObject obj)
	{
		if ((bool)jointedRigidBody && jointedRigidBody.gameObject == obj)
		{
			UnityEngine.Object.Destroy(GetComponent<FixedJoint>());
			jointedRigidBody = null;
			rb.AddForce(Vector3.up * 10f);
			rb.AddTorque(Vector3.one * 10f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 10 && other.attachedRigidbody.isKinematic && rb.velocity.sqrMagnitude < 0.25f)
		{
			//other.GetComponent<IKickable<Vector3>>().Kick(other.transform.forward);
			rb.AddForce((Vector3.up - other.transform.forward).normalized * 5f);
			rb.AddTorque(new Vector3(45f, 90f, 0f));
			if ((bool)source)
			{
				//source.PlayClip(sfxGroundedKick);
			}
			//QuickEffectsPool.Get("Poof", base.t.position, base.t.rotation).Play();
			//StylePointsCounter.instance.AddStylePoint(StylePointTypes.TrippedOver);
		}
	}

	// public virtual void PinTheBody(Body body)
	// {
	// 	jointedBody = body;
	// 	joint = base.gameObject.AddComponent<FixedJoint>();
	// 	joint.connectedBody = body.rb;
	// 	joint.connectedMassScale = 0.05f;
	// }

	// public virtual void PinTheBodyToTheWall(Body body)
	// {
	// 	jointedBody = body;
	// 	base.rb.isKinematic = true;
	// }

	// public virtual void StuckInObject(Collider c)
	// {
	// 	Body.lastBody = c.transform;
	// 	jointedRigidBody = c.attachedRigidbody;
	// 	joint = base.gameObject.AddComponent<FixedJoint>();
	// 	joint.connectedBody = jointedRigidBody;
	// 	joint.massScale = 0.05f;
	// }

	public virtual void Drop(Vector3 force, float torque = 0f)
	{
		rb.AddForce(force, ForceMode.Impulse);
		if (torque != 0f)
		{
			rb.AddTorque(-t.right * torque, ForceMode.Impulse);
		}
	}

	public virtual void Interact(WeaponManager manager)
	{   

        if(manager.currentWeapon != weaponIndex){
            manager.PickWeapon(weaponIndex);
        }
        gameObject.SetActive(false);
        Destroy(gameObject);

		if (!associatedScriptableObject)// || manager.currentWeapon != associatedScriptableObject.index)
		{
			// if ((bool)jointedBody)
			// {
			// 	jointedBody.rbs[jointedBody.rbIndex].AddForce(-base.t.up * 30f, ForceMode.Impulse);
			// 	jointedBody = null;
			// 	QuickEffectsPool.Get("Damage", base.t.position, Quaternion.identity).Play();
			// }
			// if ((bool)jointedRigidBody)
			// {
			// 	jointedRigidBody.AddForce(Vector3.up * 10f, ForceMode.Impulse);
			// }
			// if ((bool)associatedScriptableObject)
			// {
			// 	manager.PickWeapon(associatedScriptableObject.index);
			// }
			// else
			// {
			// 	manager.Pick(-1);
			// }
			// QuickEffectsPool.Get("Poof", base.t.position).Play();
			// base.gameObject.SetActive(false);
		}
		// else
		// {
		// 	if (!rb.isKinematic)
		// 	{
		// 		rb.AddForce(Vector3.up * 5f);
        //         rb.AddTorque(t.forward * 10f);
		// 	}
		// 	if ((bool)source)
		// 	{
		// 		//source.PlayClip(sfxFailedPick);
		// 	}
		// 	//QuickEffectsPool.Get("Poof", base.t.position, base.t.rotation).Play();
		// }
	}
}
