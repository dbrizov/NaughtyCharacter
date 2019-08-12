using UnityEngine;

namespace NaughtyCharacter
{
	public class SpringArm : MonoBehaviour
	{
		public float TargetLength = 3.0f;
		public float SpeedDamp = 0.0f;
		public Transform CollisionSocket;
		public float CollisionRadius = 0.25f;
		public LayerMask CollisionMask = 0;
		public Camera Camera;
		public float CameraViewportExtentsMultipllier = 1.0f;

		private Vector3 _socketVelocity;

		private void LateUpdate()
		{
			if (Camera != null)
			{
				CollisionRadius = GetCollisionRadiusForCamera(Camera);
				Camera.transform.localPosition = -Vector3.forward * Camera.nearClipPlane;
			}

			UpdateLength();
		}

		private float GetCollisionRadiusForCamera(Camera cam)
		{
			float halfFOV = (cam.fieldOfView / 2.0f) * Mathf.Deg2Rad; // vertical FOV in radians
			float nearClipPlaneHalfHeight = Mathf.Tan(halfFOV) * cam.nearClipPlane * CameraViewportExtentsMultipllier;
			float nearClipPlaneHalfWidth = nearClipPlaneHalfHeight * cam.aspect;
			float collisionRadius = new Vector2(nearClipPlaneHalfWidth, nearClipPlaneHalfHeight).magnitude; // Pythagoras

			return collisionRadius;
		}

		private float GetDesiredTargetLength()
		{
			Ray ray = new Ray(transform.position, -transform.forward);
			RaycastHit hit;

			if (Physics.SphereCast(ray, Mathf.Max(0.001f, CollisionRadius), out hit, TargetLength, CollisionMask))
			{
				return hit.distance;
			}
			else
			{
				return TargetLength;
			}
		}

		private void UpdateLength()
		{
			float targetLength = GetDesiredTargetLength();
			Vector3 newSocketLocalPosition = -Vector3.forward * targetLength;

			CollisionSocket.localPosition = Vector3.SmoothDamp(
				CollisionSocket.localPosition, newSocketLocalPosition, ref _socketVelocity, SpeedDamp);
		}

		private void OnDrawGizmos()
		{
			if (CollisionSocket != null)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine(transform.position, CollisionSocket.transform.position);
				DrawGizmoSphere(CollisionSocket.transform.position, CollisionRadius);
			}
		}

		private void DrawGizmoSphere(Vector3 pos, float radius)
		{
			Quaternion rot = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

			int alphaSteps = 8;
			int betaSteps = 16;

			float deltaAlpha = Mathf.PI / alphaSteps;
			float deltaBeta = 2.0f * Mathf.PI / betaSteps;

			for (int a = 0; a < alphaSteps; a++)
			{
				for (int b = 0; b < betaSteps; b++)
				{
					float alpha = a * deltaAlpha;
					float beta = b * deltaBeta;

					Vector3 p1 = pos + rot * GetSphericalPoint(alpha, beta, radius);
					Vector3 p2 = pos + rot * GetSphericalPoint(alpha + deltaAlpha, beta, radius);
					Vector3 p3 = pos + rot * GetSphericalPoint(alpha + deltaAlpha, beta - deltaBeta, radius);

					Gizmos.DrawLine(p1, p2);
					Gizmos.DrawLine(p2, p3);
				}
			}
		}

		private Vector3 GetSphericalPoint(float alpha, float beta, float radius)
		{
			Vector3 point;
			point.x = radius * Mathf.Sin(alpha) * Mathf.Cos(beta);
			point.y = radius * Mathf.Sin(alpha) * Mathf.Sin(beta);
			point.z = radius * Mathf.Cos(alpha);

			return point;
		}
	}
}
