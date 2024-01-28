using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vfx : MonoBehaviour
{
    private float curLifeTime;
    public float maxLifeTime = 1;
    public Vector3 offset;

    private void Start()
    {
        transform.position += offset;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //deactivates the effect when time is up.
        curLifeTime = Mathf.Clamp(curLifeTime + Time.deltaTime, 0f, maxLifeTime);
        if (curLifeTime >= maxLifeTime)
        {
            Destroy(gameObject);
        }

    }
}
