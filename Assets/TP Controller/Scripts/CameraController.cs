using UnityEngine;

[RequireComponent(typeof(CameraOcclusionProtector))]
public class CameraController : MonoBehaviour
{
    // Const variables
    private const float MinCatchSpeedDamp = 0f;
    private const float MaxCatchSpeedDamp = 1f;
    private const float MinRotationSmoothing = 0f;
    private const float MaxRotationSmoothing = 30f;

    // Serializable fields
    [SerializeField]
    private Transform target = null; // The target to follow

    [SerializeField]
    [Tooltip("Should the target be found with the target tag")]
    private bool autoFindTarget = true;

    [SerializeField]
    private string targetTag = Tag.Player;

    [SerializeField]
    [Range(MinCatchSpeedDamp, MaxCatchSpeedDamp)]
    private float catchSpeedDamp = MinCatchSpeedDamp;

    [SerializeField]
    [Range(MinRotationSmoothing, MaxRotationSmoothing)]
    [Tooltip("How fast the camera rotates around the pivot")]
    private float rotationSmoothing = 15.0f;

    // private fields
    private Transform rig; // The root transform of the camera rig
    private Transform pivot; // The point at which the camera pivots around
    private Quaternion pivotTargetLocalRotation; // Controls the X Rotation (Tilt Rotation)
    private Quaternion rigTargetLocalRotation; // Controlls the Y Rotation (Look Rotation)
    private Vector3 cameraVelocity; // The velocity at which the camera moves

    protected virtual void Awake()
    {
        if (this.autoFindTarget)
        {
            this.target = GameObject.FindGameObjectWithTag(this.targetTag).transform;
        }

        this.pivot = this.transform.parent;
        this.rig = this.pivot.parent;

        this.transform.localRotation = Quaternion.identity;
    }

    protected virtual void Update()
    {
        var controlRotation = PlayerInput.Instance.MouseRotationInput();
        this.UpdateRotation(controlRotation);
    }

    protected virtual void LateUpdate()
    {
        this.FollowTarget();
    }

    public void SetDistanceToTarget(float distanceToTarget)
    {
        Vector3 cameraTargetLocalPosition = Vector3.zero;
        cameraTargetLocalPosition.z = -distanceToTarget;
        this.transform.localPosition = cameraTargetLocalPosition;
    }

    private void FollowTarget()
    {
        if (this.target == null)
        {
            return;
        }

        this.rig.position = Vector3.SmoothDamp(this.rig.position, this.target.transform.position, ref this.cameraVelocity, this.catchSpeedDamp);
    }

    private void UpdateRotation(Quaternion controlRotation)
    {
        if (this.target != null)
        {
            // Y Rotation (Look Rotation)
            this.rigTargetLocalRotation = Quaternion.Euler(0f, controlRotation.eulerAngles.y, 0f);

            // X Rotation (Tilt Rotation)
            this.pivotTargetLocalRotation = Quaternion.Euler(controlRotation.eulerAngles.x, 0f, 0f);

            if (this.rotationSmoothing > 0.0f)
            {
                this.pivot.localRotation =
                    Quaternion.Slerp(this.pivot.localRotation, this.pivotTargetLocalRotation, this.rotationSmoothing * Time.deltaTime);

                this.rig.localRotation =
                    Quaternion.Slerp(this.rig.localRotation, this.rigTargetLocalRotation, this.rotationSmoothing * Time.deltaTime);
            }
            else
            {
                this.pivot.localRotation = this.pivotTargetLocalRotation;
                this.rig.localRotation = this.rigTargetLocalRotation;
            }
        }
    }
}
