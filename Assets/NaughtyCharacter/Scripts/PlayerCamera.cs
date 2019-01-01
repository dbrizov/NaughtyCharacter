using UnityEngine;

namespace NaughtyCharacter
{
    public class PlayerCamera : MonoBehaviour
    {
        [Tooltip("How fast the camera rotates around the pivot. Value <= 0 are interpreted as instant rotation")]
        public float RotationSpeed = 0.0f;
        public Transform Rig; // The root transform of the camera rig
        public Transform Pivot; // The point at which the camera pivots around
        public Camera Camera;

        private Quaternion _pivotTargetLocalRotation; // Controls the X Rotation (Pitch Rotation)
        private Quaternion _rigTargetLocalRotation; // Controls the Y Rotation (Yaw Rotation)

        public void SetPosition(Vector3 position)
        {
            Rig.position = position;
        }

        public void SetControlRotation(Vector2 controlRotation)
        {
            // Y Rotation (Yaw Rotation)
            _rigTargetLocalRotation = Quaternion.Euler(0.0f, controlRotation.y, 0.0f);

            // X Rotation (Pitch Rotation)
            _pivotTargetLocalRotation = Quaternion.Euler(controlRotation.x, 0.0f, 0.0f);

            if (RotationSpeed > 0.0f)
            {
                Pivot.localRotation =
                    Quaternion.Slerp(Pivot.localRotation, _pivotTargetLocalRotation, RotationSpeed * Time.deltaTime);

                Rig.localRotation =
                    Quaternion.Slerp(Rig.localRotation, _rigTargetLocalRotation, RotationSpeed * Time.deltaTime);
            }
            else
            {
                Pivot.localRotation = _pivotTargetLocalRotation;
                Rig.localRotation = _rigTargetLocalRotation;
            }
        }
    }
}
