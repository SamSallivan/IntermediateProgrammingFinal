using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningProjectile : Projectile
{
    public float chainDamage = 100;
    public float chainRadius = 1;
    public float chainDelay = 1;
    public List<GameObject> chainedTargetList = new List<GameObject>();


    public override void CollisionBehavior(Collision c) {
        //Debug.Log("HIII");
    }
    public override void HitBehavior(GameObject hitObject)
    {
        chainedTargetList.Add(hitObject);
        StartCoroutine(ChainLightning(hitObject));
    }
    IEnumerator ChainLightning(GameObject sourceObject)
    {
        yield return new WaitForSeconds(chainDelay);

        GameObject targetObject = SearchChainTarget(sourceObject, chainRadius);
        if (targetObject)
        {
            Debug.Log("OKKKKKK!!!");
            Damage damage = new Damage();
            damage.dir = targetObject.transform.position - sourceObject.transform.position; 
            damage.amount = chainDamage;
            targetObject.GetComponent<Damagable>().Damage(damage);
            chainedTargetList.Add(targetObject);
            if (chainedTargetList.Count < 30)
            {
                StartCoroutine(ChainLightning(targetObject));
            }
        }
    }

    private GameObject SearchChainTarget(GameObject hitObject, float searchRange)
    {
        Collider[] colliders =  Physics.OverlapSphere(hitObject.transform.position, chainRadius, collidableLayers);

        if (colliders[0])
        {
            float distance;
            float minDistance = chainRadius;
            Collider targetCollider = new Collider();
            //gets the closest one target only
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null && !chainedTargetList.Contains(colliders[i].gameObject))
                {
                    distance = Vector3.Distance(hitObject.transform.position, colliders[i].ClosestPoint(hitObject.transform.position));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        targetCollider = colliders[i];
                    }
                }
            }
            return targetCollider.gameObject;
        }
        return null;
    }

}
