using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class InverseKinematic : MonoBehaviour
{
	public Transform a;

	public Transform b;

	public Transform c;

	[SerializeField]
	private int sign = 1;

	[SerializeField]
	private float width = 1f;

	public float dist;

	public float height;

	private Vector3 midPoint;

	public Vector3 kneeDir;

	public Vector3 kneePos;

    public GameObject knee;

	public Transform tTarget;

	public bool active;

	public bool lookInB;

	public bool debug;

	[Range(0f, 180f)]
	public float correctionAngle = 180f;

	public void LateUpdate()
	{
		if (active)
		{
			SetTarget(tTarget);
			if (debug)
			{
				Debug.DrawLine(a.position, kneePos, Color.green);
				Debug.DrawLine(kneePos, tTarget.position, Color.green);
				Debug.DrawLine(kneePos, midPoint, Color.magenta);
				Debug.DrawLine(c.position, c.position - tTarget.up * 0.2f, Color.yellow);
			}
		}
	}

	public void Reset()
	{
		tTarget.localPosition = a.localPosition - a.parent.forward * width * 2f;
		tTarget.localEulerAngles = new Vector3(180f, 0f, 0f);
	}

	public void Setup()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		if (componentsInChildren.Length >= 2)
		{
			a = componentsInChildren[0];
			b = componentsInChildren[1];
			c = componentsInChildren[2];
		}
		if (!tTarget)
		{
			Transform transform = new GameObject(string.Format("{0} IK target", a.name)).transform;
			transform.position = c.position;
			transform.rotation = c.rotation;
			if ((bool)a.parent)
			{
				transform.SetParent(a.parent);
			}
			tTarget = transform;
		}
		width = Vector3.Distance(a.position, c.position) / 2f;
		active = true;
	}

	public void SetTarget(Transform target)
	{
		dist = Vector3.Distance(a.position, target.position);
		midPoint = (a.position + target.position) / 2f;
		kneeDir = Vector3.Cross(a.position - target.position, -target.right).normalized;
		if (dist < width * 2f)
		{
			height = Mathf.Sqrt(width * width - dist * dist / 4f);
		}
		else
		{
			height = 0f;
		}
		kneePos = midPoint - kneeDir * height * sign;

        knee.transform.position = kneePos;

		a.LookAt(kneePos, kneeDir);
		if (correctionAngle != 0f)
		{
			a.Rotate(correctionAngle, 0f, 0f);
		}
		b.LookAt(target.position, kneeDir);
		if (correctionAngle != 0f)
		{
			b.Rotate(correctionAngle, 0f, 0f);
		}
		if ((bool)c)
		{
			if (!lookInB)
			{
				c.rotation = target.rotation;
			}
			else
			{
				c.rotation = Quaternion.Slerp(c.rotation, b.rotation, Time.deltaTime * 4f);
			}
		}
	}
}
