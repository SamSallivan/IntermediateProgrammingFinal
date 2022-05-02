using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour, Slappable, Damagable
{
    public NavMeshAgent agent;

    public GameObject player;

    public Transform playerTransform;

    public LayerMask groundMask, playerMask, attackMask;

    public Vector3 targetPos;
    public GameObject targetObj;
	public float health;
	public float maxHealth = 100f;
	public float kickMinDamage = 60f;
    public bool dead; 
    public bool targeted; //Whether it has targeted a proper location.
    public bool doll; //Whether it has targeted a proper location.

    public float coolDown = 10; 
    public float dollTimer = 5f; 

    public float wanderRange, sightRange, attackRange;

    public bool playerInSight, onCoolDown, playerInRange;

    public float angleToPlayer;
    public float attackDamage;

    public Animator animator;

    public List<Collider> ragdollColliders = new List<Collider>();

	private Collider[] colliders = new Collider[2];

    public float materialTimer;

    private void Awake()
    {
        SetRigdoll();
        player = GameObject.Find("Player");
        playerTransform = player.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        coolDown = 2f;
    }

    public void MaterialUpdate(){
        if (materialTimer > 0){
            materialTimer = Mathf.MoveTowards(materialTimer, 0, Time.deltaTime*2);
        }
        Color color = Color.Lerp(Color.white, Color.red, materialTimer);
        GetComponentInChildren<Renderer>().material.SetColor("_Color", color);
    }

    public void SetRigdoll()
    {
        Collider[] colliders = this .gameObject.GetComponentsInChildren<Collider>();
        foreach(Collider c in colliders){
            if(c.gameObject != this.gameObject)
            {
                c.isTrigger = true;
                c.attachedRigidbody.isKinematic = true;
                ragdollColliders.Add(c);
                c.enabled = false;
            }
        }
    }

    public void EnableRagdoll()
    {
        coolDown = 10;
        doll = true;
        dollTimer = 0f;
        foreach(Collider c in ragdollColliders){
            if(c.gameObject != this.gameObject)
            {
                c.isTrigger = false;
                c.enabled = true;
                c.attachedRigidbody.isKinematic = false;
                GetComponent<CapsuleCollider>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Animator>().enabled = false;
                GetComponent<NavMeshAgent>().enabled = false;
            }
        }
    }

    public void DisableRagdoll()
    {

        foreach(Collider c in ragdollColliders){
            if(c.gameObject != this.gameObject)
            {
                c.isTrigger = true;
                c.attachedRigidbody.isKinematic = true;
                c.enabled = false;
            }
        }

        // ragdollColliders[0].transform.localPosition = new Vector3(0, 87.62761f, 0);
        // ragdollColliders[0].transform.localRotation = Quaternion.Euler(0, -90, -81.407f);
        
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = true;
        animator.enabled = true; 
        animator.Rebind();
        animator.Update(0f);
        //animator.SetTrigger("GetUp");
        //GetComponent<NavMeshAgent>().enabled = true;
        
        coolDown = 10f;
        doll = false;

    }

    public void GetUpUpdate(){
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
    public void GetUp(){
        GetComponent<NavMeshAgent>().enabled = true;
        coolDown = 0.5f;
    }


	public Vector3 GetNavMeshPosition(Vector3 pos, float radius = 1f)
	{
        NavMeshHit navHit; 
		if (NavMesh.SamplePosition(pos, out navHit, radius, -1))
		{
			return navHit.position;
		}
		return Vector3.zero;
	}

    private void Update()
    {
        //Look for player in its spherical range.

        // RaycastHit check;
        // var rayDirection = player.transform.position - transform.position;
        // if (Physics.Raycast(transform.position, rayDirection, out check)) {
        //     if (check.transform == player) {
        //     }
        // }

        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        MaterialUpdate();

        var lookPos = (player.transform.position - transform.position).normalized;
        lookPos.y = 0;
        angleToPlayer = Vector3.Angle(transform.forward, lookPos);

        if(!doll){
            if (coolDown > 0) CoolDown();
            else if (playerInRange && angleToPlayer < 30) Attacking();
            else if (playerInSight) Chasing();
            else if (!playerInSight) Wandering();
        }
        
		if (dollTimer < 1)
		{
			dollTimer += Time.deltaTime;
		}
		else if (doll && !dead)
		{

            RaycastHit hit;
            //Physics.Raycast(ragdollColliders[0].attachedRigidbody.position, Vector3.down, out hit, 2f, 1);
            Physics.Raycast(transform.position, Vector3.down, out hit, 2f, 1);
            if (hit.distance != 0f)
            {
                Vector3 navMeshPosition = GetNavMeshPosition(hit.point, 1f);
                if (navMeshPosition.sqrMagnitude != 0f)
                {   
                    agent.Warp(navMeshPosition);
                    if (transform.eulerAngles.x <180){ //back

                        transform.rotation = Quaternion.LookRotation(-ragdollColliders[0].attachedRigidbody.transform.right);
                        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    }
                    else{ //front
                        transform.rotation = Quaternion.LookRotation(ragdollColliders[0].attachedRigidbody.transform.right);
                        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    }

                    DisableRagdoll();
                    
                }
                else
                {
                    ragdollColliders[0].attachedRigidbody.AddForce(ragdollColliders[0].attachedRigidbody.transform.forward * 5f, ForceMode.Impulse);
                    dollTimer = Mathf.Clamp(dollTimer - 0.5f, 0.1f, float.PositiveInfinity);
                }
            }
            else
            {
                ragdollColliders[0].attachedRigidbody.AddForce(ragdollColliders[0].attachedRigidbody.transform.forward * 5f, ForceMode.Impulse);
                dollTimer = Mathf.Clamp(dollTimer - 0.5f, 0.1f, float.PositiveInfinity);
            }

        }
    }

	public void Slap(Vector3 dir)
	{
        
        Vector3 tempDir;
        RaycastHit hit;

		bool flag = false;
		float num = 12f;
        EnableRagdoll();
		if (!doll)
		{
			tempDir = Vector3.ProjectOnPlane(dir, PlayerController.instance.grounder.groundNormal);
		}
		else
		{
			tempDir = Quaternion.AngleAxis(5f, PlayerController.instance.tHead.right) * dir;
		}
		for (int i = 0; i < 3; i++)
		{
			Physics.Raycast(ragdollColliders[0].attachedRigidbody.position, tempDir, out hit, num, 148481);
			if (hit.distance != 0f)
			{
				if (hit.collider.gameObject.layer != 0)
				{
					flag = true;
					//Debug.DrawLine(rb.position, hit.point, Color.green, 2f);
					break;
				}
				//Debug.DrawLine(rbs[0].position, hit.point, Color.red, 2f);
			}
			tempDir = Quaternion.AngleAxis(-10f, PlayerController.instance.tHead.right) * tempDir;
			num -= 2f;
		}
		//StopStun();
		bool flag2 = Physics.Raycast(ragdollColliders[0].attachedRigidbody.position, Vector3.down, 1f, 1);

        for (int j = 0; j < ragdollColliders.Count; j++)
        {
            //ragdollColliders[j].attachedRigidbody.velocity = tempDir.normalized * ((j == 0) ? (flag ? 18 : 3) : (flag ? 12 : 6));
            ragdollColliders[j].attachedRigidbody.velocity = tempDir.normalized * ((j == 0) ? 2 : 4);
        }
        ragdollColliders[0].attachedRigidbody.AddForce(Vector3.up * (0), ForceMode.Impulse);
        // if (lifetime > 0f)
        // {
        //     RotateBody(0f, Mathf.Sin(Time.timeSinceLevelLoad) * 60f, 0f);
        // }
		// Game.soundsManager.PlayClipAtPosition(kickSounds[UnityEngine.Random.Range(0, kickSounds.Length)], 1f, rb.position);
		// if (!enemy.dead)
		// {
		// 	PlaySound(enemy.sfxDamage);
		// }
	}
	public virtual void Damage(Damage damage)
	{
        Debug.Log(damage.amount);

        materialTimer = 1;
        Vector3 dir = damage.dir;
        dir.Normalize();

		if (dead)
		{
            if(doll){
                // ragdollColliders[0].GetComponent<Rigidbody>().AddForce(Vector3.up * 30f, ForceMode.Impulse);
				// for (int j = 0; j < ragdollColliders.Count; j++)
				// {
				// 	ragdollColliders[j].GetComponent<Rigidbody>().velocity += dir * 9;
				// }

				for (int j = 0; j < ragdollColliders.Count; j++)
				{
					ragdollColliders[j].GetComponent<Rigidbody>().velocity += dir * 5;
				}
				ragdollColliders[0].GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
				
		        ragdollColliders[0].GetComponent<Rigidbody>().MoveRotation(ragdollColliders[0].GetComponent<Rigidbody>().rotation * Quaternion.Euler(90f, 0f, 0f));

            }
			return;
		}


        if (damage.amount < kickMinDamage)
			{
				//DamageEffects Animations

				health -= damage.amount;
				// PlaySound(sfxDamage);
				// QuickEffectsPool.Get("Damage", t.position + Vector3.up * 1.5f, Quaternion.LookRotation(damage.dir)).Play();
        }
        else{

            if (doll){
            
                EnableRagdoll();

				for (int j = 0; j < ragdollColliders.Count; j++)
				{
					ragdollColliders[j].GetComponent<Rigidbody>().velocity += dir * 5;
				}
				ragdollColliders[0].GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
				
		        ragdollColliders[0].GetComponent<Rigidbody>().MoveRotation(ragdollColliders[0].GetComponent<Rigidbody>().rotation * Quaternion.Euler(90f, 0f, 0f));

            }
            else{
            
                EnableRagdoll();

                for (int j = 0; j < ragdollColliders.Count; j++)
                {
                    ragdollColliders[j].GetComponent<Rigidbody>().velocity = dir * 5;
                }
				ragdollColliders[0].GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }




            if (health - damage.amount > 0f)
            {
                health -= damage.amount;
            }
            else
            {
                health -= damage.amount;
                Die();
                dead = true;
				ragdollColliders[0].GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }
        }
	}

	private void Die()
	{
		// PlaySound(enemy.sfxDead);
		// mat.SetFloatByName("_Power", 0f);
		// DropSomething();
		// DropDead();
		GetComponent<CapsuleCollider>().enabled = false;
	}

    public void CoolDown()
    {
        if(GetComponent<Animator>().enabled)
            GetComponent<Animator>().SetBool("Running", false);
        if(agent.isActiveAndEnabled)
            agent.SetDestination(transform.position); //stops moving when on cool down.
        coolDown -= Time.deltaTime; 
    }

    public void Wandering()
    {
        //Randomizes a position within navmesh.
        if (!targeted) 
        {   
            float randomZ = Random.Range(-wanderRange, wanderRange);
            float randomX = Random.Range(-wanderRange, wanderRange);
            targetPos = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            //Checks if the position is inside of the navmesh.
            if (Physics.Raycast(targetPos, -transform.up, 2f, groundMask)){
                NavMeshHit hit;
                NavMeshPath path = new NavMeshPath();
                if(agent.enabled)
                    agent.CalculatePath(targetPos, path);
                if(NavMesh.SamplePosition(targetPos, out hit, 1f, NavMesh.AllAreas) && path.status == NavMeshPathStatus.PathComplete){
                    targeted = true;
                    //targetObj.transform.position = targetPos;
                }
            }
        }
        
        //Moves towards the position.
        if (targeted)
        {   
            if(agent.isActiveAndEnabled){
                agent.SetDestination(targetPos);
                animator.SetBool("Running", true);
            }

            //Goes on cooldown when reaches the position.
            Vector3 distanceToWalkPoint = transform.position - targetPos;
            if (distanceToWalkPoint.magnitude < 2.5f)
            {
                targeted = false;
                coolDown = Random.Range(0f, 1f);
                animator.SetBool("Running", false);
            }
        }

    }
    
    public void Chasing()
    {
        //Moves towards the player.
        //agent.enabled = true;
        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f); 

        if(agent.isActiveAndEnabled){
            agent.SetDestination(playerTransform.position);
            animator.SetBool("Running", true);
        }
    }
    
    public void Attacking()
    {        
        if(agent.isActiveAndEnabled)
            agent.SetDestination(transform.position);
        animator.SetBool("Running", false);
        animator.SetTrigger("Attack");
        coolDown = 10;
    }

    void Srike()
    {		
        var targetDir = (player.transform.position - transform.position).normalized;
        targetDir.y = 0;
        if (Vector3.Angle(transform.forward, targetDir) < 60)
		{
			transform.rotation = Quaternion.LookRotation(targetDir);
            Physics.OverlapBoxNonAlloc(transform.position + transform.forward * 1.25f, new Vector3(0.7f, 1.25f, 1.25f), colliders, transform.rotation, attackMask);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] == null)
                {
                    continue;
                }
                if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Damagable attackTarget = colliders[i].GetComponent<Damagable>();
                    if (attackTarget != null)
                    {
                        Damage damage = new Damage(attackDamage);
                        damage.dir = targetDir;
                        damage.dir.y = 0f;
                        damage.amount = attackDamage;
                        attackTarget.Damage(damage);
                    }
                }
                // else if (friendlyFire)
                // {
                //     friendlyDamageInfo.dir = (t.forward * ((t.InverseTransformPoint(colliders[i].transform.position).z > 0f) ? 1 : (-1)) + t.up) / 2f;
                //     friendlyDamageInfo.amount = 10f;
                //     friendlyDamageInfo.knockdown = true;
                //     friendlyDamageInfo.type = DamageInfo.DamageType.FriendlyFire;
                //     colliders[i].GetComponent<IDamageable<DamageInfo>>().Damage(friendlyDamageInfo);
                //     StylePointsCounter.instance.AddStylePoint(StylePointTypes.FriendlyFire);
                // }
                colliders[i] = null;
            }
		}

        //goes on cooldown, 
        coolDown = Random.Range(0.5f, 1f);
        }
    
}
