using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanController : MonoBehaviour
{

	private void Update()
	{
        transform.Rotate(Vector3.up * (360f * Time.deltaTime));
    }
}
