using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vfx_Billboard : Vfx
{
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        var lookPos = Camera.main.transform.position - transform.position;
        //lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.LookRotation(lookPos);

    }
}
