using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIndicator : MonoBehaviour
{
	public Transform tMesh;
    void Awake()
    {
		tMesh = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {   
        if((bool)tMesh){
            tMesh.localPosition = new Vector3(0f, Mathf.Sin(Time.timeSinceLevelLoad) * 0.2f, 0f);
            tMesh.Rotate(Vector3.up * (360f * Time.deltaTime));
        }
    }
}
