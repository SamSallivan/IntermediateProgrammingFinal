using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public float speed = 10;
    //layers that receives player damage.
    public LayerMask collidableLayers;
    public float hitDamage;
    private bool collidedOnce = false;

    private float curLifeTime = 0f;
    public float maxLifeTime = 5f;

    private Vector3 spawnPos;
    public float maxDistance = 15f;
    public float destroyDelay = 1;

    void Start()
    {
        spawnPos = transform.position;

        //launches the bullet in the given direction.
        GetComponent<Rigidbody>().AddForce(transform.forward * 10, ForceMode.Impulse);

        //GetComponent<Rigidbody>().velocity
        //GetComponent<Collider>().attachedRigidbody
        //rb.AddTorque(-t.right * torque, ForceMode.Impulse);

    }

    void Update()
    {
        curLifeTime = Mathf.Clamp(curLifeTime + Time.deltaTime, 0f, maxLifeTime);

        //deactivates the bullet when time is up.
        if (curLifeTime >= maxLifeTime || Vector3.Distance(spawnPos, transform.position) >= maxDistance)
        {
            StartCoroutine(DelayedDDestroy());
        }

    }

    public void OnCollisionEnter(Collision c)
    {
        if (!collidedOnce)
        {
            collidedOnce = true;
            if (collidableLayers == (collidableLayers | (1 << c.gameObject.layer)))
            {
                Damage damage = new Damage();
                damage.dir = (-c.contacts[0].normal + Vector3.up) / 2f;
                damage.amount = hitDamage;
                //damage.amount = c.relativeVelocity.magnitude * hitDamage;
                c.gameObject.GetComponent<Damagable>().Damage(damage);

                HitBehavior(c.gameObject);

                /*PlayerController pc = FindObjectOfType<PlayerController>();
                pc.slamVFX.transform.position = c.transform.position;
                pc.slamVFX.transform.rotation = Quaternion.LookRotation(c.transform.forward);
                pc.slamVFX.GetComponent<ParticleSystem>().Play();*/

            }

            CollisionBehavior(c);

            StartCoroutine(DelayedDDestroy());
        }
    }

    public virtual void HitBehavior(GameObject hitObject) { }
    public virtual void CollisionBehavior(Collision c) { }
    IEnumerator DelayedDDestroy()
    {
        yield return new WaitForSeconds(destroyDelay);
        
        Destroy(gameObject);
    }
}
