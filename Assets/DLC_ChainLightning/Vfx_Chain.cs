using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vfx_Chain : Vfx
{
    private GameObject startTarget;
    private GameObject endTarget;
    public List<LineRenderer> lineRenderers;


    public void Initiate(GameObject start, GameObject end)
    {
        startTarget = start;
        endTarget = end;
        foreach (LineRenderer line in lineRenderers)
        {
            line.SetPosition(0, startTarget.transform.position + offset);
            line.SetPosition(1, endTarget.transform.position + offset);
        }
    }

    /*public override void Update()
    {
        base.Update();
        if (startTarget && endTarget) {
            lineRenderer.SetPosition(0, startTarget.transform.position + offset);
            lineRenderer.SetPosition(1, endTarget.transform.position + offset);
        }
    }*/
}
