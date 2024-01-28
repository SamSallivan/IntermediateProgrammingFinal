using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_ChainLightning : MonoBehaviour
{
    public Settings_ChainLightning settings;
    
    private List<Enemy> historyTargetList = new List<Enemy>();

    public void Initialize(Settings_ChainLightning settings, Enemy sourceEnemy)
    {
        this.settings = settings;
        Instantiate(settings.hitVfx, sourceEnemy.transform.position, Quaternion.identity);
        historyTargetList.Add(sourceEnemy);
        StartCoroutine(ChainLightning(sourceEnemy));
    }


    IEnumerator ChainLightning(Enemy sourceEnemy)
    {
        yield return new WaitForSeconds(settings.chainDelay);

        Enemy targetEnemy = SearchChainTarget(sourceEnemy, settings.chainRadius);

        if (targetEnemy && historyTargetList.Count < settings.maxChainTargetCount)
        {
            Damage damage = new Damage();
            damage.dir = targetEnemy.transform.position - sourceEnemy.transform.position;
            damage.dir = Vector3.zero;
            damage.amount = settings.chainDamage;
            targetEnemy.GetComponent<Damagable>().Damage(damage);
            Instantiate(settings.hitVfx, targetEnemy.transform.position, Quaternion.identity);
            Vfx_Chain chain = Instantiate(settings.chainVfx).GetComponent<Vfx_Chain>();
            chain.Initiate(sourceEnemy.gameObject,targetEnemy.gameObject);
            historyTargetList.Add(targetEnemy);
            StartCoroutine(ChainLightning(targetEnemy));
            //Debug.Log(historyTargetList.Count + ": " + targetEnemy.name);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Enemy SearchChainTarget(Enemy sourceEnemy, float searchRange)
    {
        Collider[] colliders = Physics.OverlapSphere(sourceEnemy.transform.position, settings.chainRadius, settings.searchLayers);

        if (colliders.Length > 0)
        {
            float distance;
            float minDistance = settings.chainRadius;
            Enemy targetEnemy = null;

            switch (settings.chainClosestTarget)
            {
                case true:

                    //gets the closest one target only
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        Enemy curEnemy = colliders[i].GetComponent<Enemy>();
                        if (curEnemy && !historyTargetList.Contains(curEnemy))
                        {
                            distance = Vector3.Distance(sourceEnemy.transform.position, colliders[i].ClosestPoint(sourceEnemy.transform.position));
                            if (distance <= minDistance)
                            {
                                minDistance = distance;
                                targetEnemy = curEnemy;
                            }
                        }
                    }

                    return targetEnemy;

                case false:

                    //gets random not contained in list
                    List<Enemy> curTargetList = new List<Enemy>();
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        Enemy curEnemy = colliders[i].GetComponent<Enemy>();
                        if (curEnemy && !historyTargetList.Contains(curEnemy))
                        {
                            curTargetList.Add(curEnemy);
                        }
                    }
                    if (curTargetList.Count > 0) {
                        targetEnemy = curTargetList[Random.Range(0, curTargetList.Count - 1)];
                    }
                    return targetEnemy;

            }
        }

        return null;
    }
}
