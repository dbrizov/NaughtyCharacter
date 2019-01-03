using UnityEngine;

namespace NaughtyCharacter
{
    public class SpringArm : MonoBehaviour
    {
        public float TargetLength = 3.0f;
        public float SpeedDamp = 0.025f;
        public Transform CollisionSocket;
        public float CollisionRadius;
        public LayerMask CollisionMask = 0;

        private Vector3 _socketVelocity;

        private void LateUpdate()
        {
            UpdateLength();
        }

        private float GetDesiredTargetLength()
        {
            Ray ray = new Ray(transform.position, -transform.forward);
            RaycastHit hit;

            if (Physics.SphereCast(ray, Mathf.Max(0.001f, CollisionRadius), out hit, TargetLength, CollisionMask))
            {
                return hit.distance; // + CollisionRadius;
            }
            else
            {
                return TargetLength;
            }
        }

        private void UpdateLength()
        {
            float targetLength = GetDesiredTargetLength();
            Debug.Log(targetLength);
            Vector3 newSocketLocalPosition = Vector3.back * targetLength;

            CollisionSocket.localPosition = Vector3.SmoothDamp(
                CollisionSocket.localPosition, newSocketLocalPosition, ref _socketVelocity, SpeedDamp);
        }
    }
}
