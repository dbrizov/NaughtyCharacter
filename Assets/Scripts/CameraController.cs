using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public const float MinDistanceToPlayer = 1f;
    public const float MaxDistanceToPlayer = 5f;
    public const float MinCatchSpeedDamp = 0f;
    public const float MaxCatchSpeedDamp = 1f;
    public const float MinRotationSmoothing = 0f;
    public const float MaxRotationSmoothing = 30f;

    [SerializeField]
    private Character target = null; // The target to follow

    [SerializeField]
    [Tooltip("Should the target be found with the target tag")]
    private bool autoFindTarget = true;

    [SerializeField]
    private string targetTag = Tag.Player;

    [SerializeField]
    [Range(MinDistanceToPlayer, MaxDistanceToPlayer)]
    [Tooltip("In meters")]
    private float distanceToTarget = 2.5f; // In meters

    [SerializeField]
    [Range(MinCatchSpeedDamp, MaxCatchSpeedDamp)]
    private float catchSpeedDamp = MinCatchSpeedDamp;

    [SerializeField]
    [Range(MinRotationSmoothing, MaxRotationSmoothing)]
    [Tooltip("How fast the camera rotates around the pivot")]
    private float rotationSmoothing = 15.0f;

    private Transform rig; // The root transform of the camera rig
    private Transform pivot; // The pivot the camera is rotating around
    private Quaternion pivotTargetLocalRotation; // Controls the X Rotation (Tilt Rotation)
    private Quaternion rigTargetLocalRotation; // Controlls the Y Rotation (Look Rotation)
    private Vector3 cameraVelocity; // The velocity at which the camera moves

    protected virtual void Awake()
    {
        if (this.autoFindTarget)
        {
            this.target = GameObject.FindGameObjectWithTag(this.targetTag).GetComponent<Character>();
        }

        this.pivot = this.transform.parent;
        this.rig = this.pivot.parent;

        // Position the camera
        Vector3 cameraTargetLocalPosition = Vector3.zero;
        cameraTargetLocalPosition.z = -this.distanceToTarget;
        this.transform.localPosition = cameraTargetLocalPosition;
    }

    protected virtual void LateUpdate()
    {
        this.FollowTarget();
        this.UpdateRotation(Time.deltaTime);
    }

    private void FollowTarget()
    {
        if (this.target == null)
        {
            return;
        }

        this.rig.position = Vector3.SmoothDamp(this.rig.position, this.target.transform.position, ref this.cameraVelocity, this.catchSpeedDamp);
    }

    private void UpdateRotation(float deltaTime)
    {
        if (this.target != null)
        {
            // Y Rotation (Look Rotation)
            this.rigTargetLocalRotation = this.target.ControlRotationY;

            // X Rotation (Tilt Rotation)
            this.pivotTargetLocalRotation = this.target.ControlRotationX;

            if (this.rotationSmoothing > 0.0f)
            {
                this.pivot.localRotation = Quaternion.Slerp(this.pivot.localRotation, this.pivotTargetLocalRotation, this.rotationSmoothing * deltaTime);
                this.rig.localRotation = Quaternion.Slerp(this.rig.localRotation, this.rigTargetLocalRotation, this.rotationSmoothing * deltaTime);
            }
            else
            {
                this.pivot.localRotation = this.pivotTargetLocalRotation;
                this.rig.localRotation = this.rigTargetLocalRotation;
            }
        }
    }
}
