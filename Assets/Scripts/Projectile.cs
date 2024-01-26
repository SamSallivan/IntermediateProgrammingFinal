using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    //layers that receives player damage.
    public LayerMask collidableLayers;

    private Damage damage = new Damage();
    public float damageAmount;

    [SerializeField]
    private float curLifeTime = 0f;
    public float maxLifeTime = 5f;
    public float maxDistance = 15f;
    private Vector3 spawnPos;

    void Start()
    {
        spawnPos = transform.position;

        //launches the bullet towards the given direction.
        GetComponent<Rigidbody>().AddForce(transform.forward * 10, ForceMode.Impulse);

        //GetComponent<Rigidbody>().velocity
        //GetComponent<Collider>().attachedRigidbody
        //rb.AddTorque(-t.right * torque, ForceMode.Impulse);

    }

    // Update is called once per frame
    void Update()
    {
        curLifeTime = Mathf.Clamp(curLifeTime + Time.deltaTime, 0f, maxLifeTime);

        //deactivates the bullet when time is up.
        if (curLifeTime >= maxLifeTime || Vector3.Distance(spawnPos, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }

        //transform.position = Vector3.Lerp(transform.position, transform.position + dir, time * 2 / maxTime);

    }

    public void OnCollisionEnter(Collision c)
    {
        if (collidableLayers == (collidableLayers | (1 << c.gameObject.layer)))
        {
            damage.dir = (-c.contacts[0].normal + Vector3.up) / 2f;
            damage.amount = c.relativeVelocity.magnitude * damageAmount;
            c.gameObject.GetComponent<Damagable>().Damage(damage);

            HitBehavior(c.gameObject);

            /*PlayerController pc = FindObjectOfType<PlayerController>();
            pc.slamVFX.transform.position = c.transform.position;
            pc.slamVFX.transform.rotation = Quaternion.LookRotation(c.transform.forward);
            pc.slamVFX.GetComponent<ParticleSystem>().Play();*/

        }

        CollisionBehavior(c);

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        //Destroy(gameObject);
    }

    public virtual void HitBehavior(GameObject hitObject) { }
    public virtual void CollisionBehavior(Collision c) { }
}
