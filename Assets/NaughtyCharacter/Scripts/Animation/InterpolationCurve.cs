using UnityEngine;

namespace NaughtyCharacter
{
	[CreateAssetMenu(fileName = "InterpolationCurve", menuName = "NaughtyCharacter/InterpolationCurve")]
	public class InterpolationCurve : ScriptableObject
	{
		public AnimationCurve Curve;

		public float Evaluate(float time)
		{
			return Curve.Evaluate(time);
		}

		public float Interpolate(float from, float to, float time)
		{
			return from + (to - from) * Evaluate(time);
		}

		public Vector2 Interpolate(Vector2 from, Vector2 to, float time)
		{
			return from + (to - from) * Evaluate(time);
		}

		public Vector3 Interpolate(Vector3 from, Vector3 to, float time)
		{
			return from + (to - from) * Evaluate(time);
		}

		public Color Interpolate(Color from, Color to, float time)
		{
			return from + (to - from) * Evaluate(time);
		}
	}
}
