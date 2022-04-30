using UnityEngine;

public static class Vector3Extensions
{
	public static Vector3 Snap(this Vector3 v)
	{
		v.x = Mathf.Round(v.x);
		v.y = Mathf.Round(v.y);
		v.z = Mathf.Round(v.z);
		return v;
	}

	public static Vector3 Snap(this Vector3 v, float step = 1f)
	{
		v.x = Mathf.Round(v.x / step) * step;
		v.y = Mathf.Round(v.y / step) * step;
		v.z = Mathf.Round(v.z / step) * step;
		return v;
	}

	public static Vector3 DirTo(this Vector3 v, Vector3 target)
	{
		return (target - v).normalized;
	}

	public static Vector3 ClosestToMe(this Vector3 v, Vector3[] points)
	{
		Vector3 result = default(Vector3);
		float num = float.PositiveInfinity;
		foreach (Vector3 vector in points)
		{
			float sqrMagnitude = (vector - v).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = vector;
			}
		}
		return result;
	}

	public static float PerlinNoiseFromPosition(this Vector3 v, int seed, float frequency = 0.1f, float amplitude = 1f, float persistence = 1f, int octave = 1)
	{
		float num = 0f;
		for (int i = 0; i < octave; i++)
		{
			float num2 = Mathf.PerlinNoise(v.x * frequency + (float)seed, v.y * frequency + (float)seed) * amplitude;
			float num3 = Mathf.PerlinNoise(v.x * frequency + (float)seed, v.z * frequency + (float)seed) * amplitude;
			float num4 = Mathf.PerlinNoise(v.y * frequency + (float)seed, v.z * frequency + (float)seed) * amplitude;
			float num5 = Mathf.PerlinNoise(v.y * frequency + (float)seed, v.x * frequency + (float)seed) * amplitude;
			float num6 = Mathf.PerlinNoise(v.z * frequency + (float)seed, v.x * frequency + (float)seed) * amplitude;
			float num7 = Mathf.PerlinNoise(v.z * frequency + (float)seed, v.y * frequency + (float)seed) * amplitude;
			num += (num2 + num3 + num4 + num5 + num6 + num7) / 6f;
			amplitude *= persistence;
			frequency *= 2f;
		}
		return num / (float)octave;
	}

	public static int SideFromDirection(this Vector3 fwd, Vector3 dir, Vector3 normal)
	{
		float num = Vector3.Dot(Vector3.Cross(fwd, dir), normal);
		if (num > 0f)
		{
			return 1;
		}
		if (num < 0f)
		{
			return -1;
		}
		return 0;
	}

	public static int IndexOfClosestToMe(this Vector3 v, Vector3[] points)
	{
		int result = 0;
		float num = float.PositiveInfinity;
		for (int i = 0; i < points.Length; i++)
		{
			float sqrMagnitude = (points[i] - v).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = i;
			}
		}
		return result;
	}

	public static Vector3 Reset(this Vector3 v, float? x = null, float? y = null, float? z = null)
	{
		v.x = x ?? 0f;
		v.y = y ?? 0f;
		v.z = z ?? 0f;
		return v;
	}

	public static Vector4 Reset(this Vector4 v, float? x = null, float? y = null, float? z = null, float? w = null)
	{
		v.x = x ?? 0f;
		v.y = y ?? 0f;
		v.z = z ?? 0f;
		v.w = w ?? 0f;
		return v;
	}

	public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
	{
		return new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
	}

	public static Vector4 With(this Vector4 v, float? x = null, float? y = null, float? z = null, float? w = null)
	{
		v.x = x ?? v.x;
		v.y = y ?? v.y;
		v.z = z ?? v.z;
		v.w = w ?? v.w;
		return v;
	}

	public static Vector2 With(this Vector2 v, float? x = null, float? y = null)
	{
		return new Vector2(x ?? v.x, y ?? v.y);
	}

	public static Vector2 Cross(this Vector2 v)
	{
		v = new Vector2(v.y, 0f - v.x);
		return v;
	}

	public static Vector3 Mid(this Vector3 a, Vector3 b, float distance = 0.5f)
	{
		return (a + b) * distance;
	}

	public static Vector2 Mid(this Vector2 a, Vector2 b, float distance = 0.5f)
	{
		return (a + b) * distance;
	}

	public static Vector3 RotateVector(this Vector3 v, float? x = null, float? y = null, float? z = null)
	{
		v = Quaternion.Euler(x ?? 0f, y ?? 0f, z ?? 0f) * v;
		return v;
	}

	public static Quaternion Rotation2D(this Vector2 vector)
	{
		return Quaternion.AngleAxis(Mathf.Atan2(vector.y, vector.x) * 57.29578f, Vector3.forward);
	}

	public static Quaternion Rotate(this Quaternion quaternion, Quaternion target, float speed = 1f)
	{
		quaternion = Quaternion.RotateTowards(quaternion, target, Time.deltaTime * speed * 360f);
		return quaternion;
	}

	public static Quaternion Quaternionise(this Vector3 v)
	{
		return Quaternion.LookRotation(v);
	}

	public static Vector2 WorldToScreenSpace(this Vector3 v, Camera cam, RectTransform area)
	{
		Vector3 vector = cam.WorldToScreenPoint(v);
		vector.z = 0f;
		Vector2 localPoint;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(area, vector, cam, out localPoint))
		{
			return localPoint;
		}
		return vector;
	}
}
