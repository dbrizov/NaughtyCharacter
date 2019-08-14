using UnityEngine;

namespace NaughtyCharacter
{
	public static class Util
	{
		public static Vector3 SetX(this Vector3 vec, float x)
		{
			return new Vector3(x, vec.y, vec.z);
		}

		public static Vector3 SetY(this Vector3 vec, float y)
		{
			return new Vector3(vec.x, y, vec.z);
		}

		public static Vector3 SetZ(this Vector3 vec, float z)
		{
			return new Vector3(vec.x, vec.y, z);
		}

		public static Vector3 Multiply(this Vector3 vec, float x, float y, float z)
		{
			return new Vector3(vec.x * x, vec.y * y, vec.z * z);
		}

		public static Vector3 Multiply(this Vector3 vec, Vector3 other)
		{
			return Multiply(vec, other.x, other.y, other.z);
		}

		public static Vector3 Clamp(this Vector3 vec, Vector3 min, Vector3 max)
		{
			vec.x = Mathf.Clamp(vec.x, min.x, max.x);
			vec.y = Mathf.Clamp(vec.y, min.y, max.y);
			vec.z = Mathf.Clamp(vec.z, min.z, max.z);

			return vec;
		}

		public static float Remap(this float f, float fromMin, float fromMax, float toMin, float toMax)
		{
			float t = (f - fromMin) / (fromMax - fromMin);
			return Mathf.LerpUnclamped(toMin, toMax, t);
		}
	}
}
