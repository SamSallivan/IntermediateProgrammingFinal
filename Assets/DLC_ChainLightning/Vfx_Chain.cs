using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Vfx_Chain : Vfx
{
    private LineRenderer lineRenderer;
    private GameObject startTarget;
    private GameObject endTarget;


    public void Initiate(GameObject start, GameObject end)
    {
        lineRenderer = GetComponent<LineRenderer>();
        startTarget = start;
        endTarget = end;
        lineRenderer.SetPosition(0, startTarget.transform.position + offset);
        lineRenderer.SetPosition(1, endTarget.transform.position + offset);
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
