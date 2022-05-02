using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFracture : MonoBehaviour, Slappable, Damagable
{

	public float maxHealth = 100f;

	[Header("Damage")]
	public float minDamageVelocitySqr = 25f;

	public LayerMask damageMask;

	public Damage damage = new Damage();

	public AudioClip sound;

	public GameObject onBreakPrefab;

	//public bool canHurtPlayer;

	private Transform t;

	private AudioSource source;

	private float stunTimer;

	private Vector3 stunGravity = new Vector3(0f, -2f, 0f);

	private Vector3 startPosition;

	private Quaternion startRotation;

	public bool lethal;

	public bool broken;

	public float health;

	public Rigidbody rb { get; private set; }

	public Collider clldr { get; private set; }

    public float materialTimer;
    public bool sleep;

	public EnemyMove[] activateEnemies;
    public bool roomCleared;

	public void Slap(Vector3 dir)
	{
		if(activateEnemies.Length!=0){
			foreach(EnemyMove enemy in activateEnemies){
				enemy.gameObject.SetActive(true);
			}
		}

		rb.isKinematic = false;
		if (stunTimer != 0f)
		{
			Unstun();
		}
		if ((bool)rb && !rb.isKinematic)
		{
			if (dir == Vector3.up)
			{
				rb.AddForce(dir.normalized * 15f, ForceMode.Impulse);
			}
			else
			{		
				

				//EnemyMove[] enemies = FindObjectsOfType<EnemyMove>(true);
				foreach (EnemyMove enemy in activateEnemies)
				{
					if (enemy.dead)
					{
						continue;
					}

					Vector3 direction = (enemy.transform.position - transform.position).normalized;

					float distance = Vector3.Distance(transform.position, enemy.transform.position);
					float leastAngle = 30;

					if (!(distance > 15) && !Physics.Raycast(transform.position, direction, distance, 1))
					{
						if (Vector3.Angle(direction, dir) < leastAngle)
						{
							leastAngle = Vector3.Angle(direction, dir);
							dir = direction;
						}
					}
				}

				rb.AddForce(dir.normalized * 45f, ForceMode.Impulse);
				//rb.AddForce(CrowdControl.instance.GetClosestDirectionToNormal(rb.worldCenterOfMass, dir, 30f) * 45f, ForceMode.Impulse);
			}
		}
		//PlaySound(effect.kickSound);
	}

	public void Damage(Damage damage)
	{
		
		if(activateEnemies.Length!=0){
			foreach(EnemyMove enemy in activateEnemies){
				enemy.gameObject.SetActive(true);
			}
		}

		rb.isKinematic = false;

		materialTimer = 1;
		health -= damage.amount;

		// if (damage.type == DamageInfo.DamageType.StunUp || damage.type == DamageInfo.DamageType.SlideBash)
		// {
		// 	stunTimer = 4f;
		// 	rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
		// 	rb.drag = 2f;
		// 	rb.useGravity = false;
		// 	blinker.Stun();
		// }

		// else if (stunTimer != 0f)
		// {
		// 	Unstun();
		// }

		if ((bool)rb && !rb.isKinematic && damage.amount > 0f)
		{
			Vector3 dir = damage.dir;
			dir *= Mathf.Clamp(damage.amount, 0f, 50f);
			rb.AddForce(dir, ForceMode.Impulse);
			rb.AddTorque(dir, ForceMode.Impulse);
		}

		if (health > 0f)
		{
			//PlaySound(effect.damage[1]);
		}
		else if (!broken)
		{
			Break(damage.dir);
		}
	}

	private void Break(Vector3 dir)
	{	
        broken = true;

		if(onBreakPrefab != null){

			GetComponent<MeshRenderer>().enabled = false;
			GetComponent<MeshCollider>().enabled = false;

			if (gameObject.activeInHierarchy)
			{
				if (onBreakPrefab != null)
				{
					GameObject temp = GameObject.Instantiate(onBreakPrefab, rb.worldCenterOfMass, t.rotation);
					

					foreach (Transform t in temp.transform)
					{
						var rb = t.GetComponent<Rigidbody>();

						if (rb != null)
							rb.AddExplosionForce(Random.Range(0, 5), transform.position, Random.Range(5, 20));

						StartCoroutine(Shrink(t, 2));
					}

				}
				//base.gameObject.SetActive(false);
			}
		}
	}

    IEnumerator Shrink (Transform t, float delay)
    {
        yield return new WaitForSeconds(delay);

        while(t.localScale.x >= 0.05f)
        {

            t.localScale = t.localScale * 9/10;

            yield return new WaitForSeconds (0.05f);
        }

        Destroy(t.gameObject);

        yield return new WaitForSeconds(delay);

        //gameObject.SetActive(false);
    }

	private void Awake()
	{
		t = base.transform;
		source = GetComponentInChildren<AudioSource>();
		rb = GetComponent<Rigidbody>();
		clldr = GetComponent<Collider>();
		startPosition = t.position;
		startRotation = t.rotation;
		if(sleep){
			rb.isKinematic = true;
		}
	}

	private void Unstun()
	{
		stunTimer = 0f;
		rb.drag = 0f;
		rb.useGravity = true;
	}


    public void MaterialUpdate(){
        if (materialTimer > 0){
            materialTimer = Mathf.MoveTowards(materialTimer, 0, Time.deltaTime*2);
        }
        Color color = Color.Lerp(Color.white, Color.red, materialTimer);
        GetComponentInChildren<Renderer>().material.SetColor("_Color", color);
    }

	private void Update()
	{
		MaterialUpdate();
		if (stunTimer > 0f)
		{
			stunTimer = Mathf.MoveTowards(stunTimer, 0f, Time.deltaTime);
			if (stunTimer == 0f)
			{
				Unstun();
			}
		}
		lethal = rb.velocity.sqrMagnitude > minDamageVelocitySqr;
		
		if(!roomCleared){
			int deathCount = 0;
			foreach(EnemyMove enemy in activateEnemies){
				if(enemy.dead){
					deathCount++;
				}
			}
			if(deathCount == activateEnemies.Length && !roomCleared){
				roomCleared = true;
				TimeManager.instance.SlowMotion(0.2f, 0.6f, 0.1f);
			}
		}
	}

	private void FixedUpdate()
	{
		if (stunTimer > 0f)
		{
			rb.AddTorque(Vector3.one * (180f * Time.deltaTime));
			rb.AddForce(stunGravity);
		}
	}

	private void OnEnable()
	{
		health = maxHealth;
	}

	private void Reset()
	{
		health = maxHealth;
		t.SetPositionAndRotation(startPosition, startRotation);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		source.Stop();
		if (stunTimer != 0f)
		{
			Unstun();
		}
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(true);
		}
	}

	private void OnCollisionEnter(Collision c)
	{
		float sqrMagnitude = c.relativeVelocity.sqrMagnitude;
		int layer = c.gameObject.layer;
		if (minDamageVelocitySqr != 0f && lethal && (int)damageMask == ((int)damageMask | (1 << layer)))
		{
            damage.dir = (-c.contacts[0].normal + Vector3.up) / 2f;
            damage.amount = c.relativeVelocity.magnitude * 3f;

            if (c.gameObject.activeInHierarchy)
            {
                c.transform.GetComponentInChildren<Damagable>().Damage(damage);
				Debug.Log(c.gameObject.name);
				
				PlayerController pc = FindObjectOfType<PlayerController>();
				pc.slamVFX.transform.position = c.transform.position;
				pc.slamVFX.transform.rotation = Quaternion.LookRotation(c.transform.forward);
				pc.slamVFX.GetComponent<ParticleSystem>().Play();
            }
            rb.velocity = Vector3.zero;
            rb.AddForce(c.contacts[0].normal * c.relativeVelocity.magnitude);
            //Game.soundsManager.PlayClipAtPosition(sound, 1f, base.transform.position);
            if (layer == 10)
            {
                health -= sqrMagnitude;
                if (health <= 0f)
                {
                    Break(c.contacts[0].normal);
                }
                //CameraController.shake.Shake(1);
            }
            else
            {
                health -= c.relativeVelocity.magnitude;
                if (health <= 0f)
                {
                    Break(c.contacts[0].normal);
                }
            }
		}
	}

	private void PlaySound(AudioClip clip)
	{
		if (base.isActiveAndEnabled)
		{
			if (source.isPlaying)
			{
				source.Stop();
			}
			if (source.clip != clip)
			{
				source.clip = clip;
			}
			source.Play();
		}
	}
}
