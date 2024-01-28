using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_ChainLightning : Projectile
{
    public Settings_ChainLightning chainLightningSettings;

    public override void HitBehavior(GameObject hitObject)
    {
        Enemy hitEnemy = hitObject.GetComponent<Enemy>();
        if (hitEnemy)
        {
            Actor_ChainLightning onHitSpawnedActor = new GameObject("Actor_ChainLightning").AddComponent<Actor_ChainLightning>();
            onHitSpawnedActor.Initialize(chainLightningSettings, hitEnemy);
        }
    }

    public override void CollisionBehavior(Collision c)
    {

    }

}

[System.Serializable]
public class Settings_ChainLightning
{
    public float chainDamage;
    public float chainRadius;
    public float chainDelay;
    public int maxChainTargetCount;
    public LayerMask searchLayers;
    public bool chainClosestTarget;
    public GameObject chainVfx;
    public GameObject hitVfx;

}
