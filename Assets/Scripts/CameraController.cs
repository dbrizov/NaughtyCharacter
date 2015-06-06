using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public const float MinTiltAngle = -89.0f;
    public const float MaxTiltAngle = 89.0f;
    public const float MinDistanceToPlayer = 1f;
    public const float MaxDistanceToPlayer = 5f;
    public const float MinCatchSpeedDamp = 0f;
    public const float MaxCatchSpeedDamp = 1f;
    public const float MinMouseSensitivity = 1f;
    public const float MaxMouseSensitivity = 5f;
    public const float MinRotationSmoothing = 0f;
    public const float MaxRotationSmoothing = 30f;

    [SerializeField]
    private Transform target = null; // The target to follow

    [SerializeField]
    [Tooltip("Should the target be found with the target tag")]
    private bool autoFindTarget = true;

    [SerializeField]
    private string targetTag = Tag.Player;

    [SerializeField]
    [Range(MinDistanceToPlayer, MaxDistanceToPlayer)]
    [Tooltip("In meters")]
    private float distanceToTarget = 3.0f; // In meters

    [SerializeField]
    [Range(MinCatchSpeedDamp, MaxCatchSpeedDamp)]
    private float catchSpeedDamp = 0.1f;

    [SerializeField]
    [Range(MinMouseSensitivity, MaxMouseSensitivity)]
    private float mouseSensitivity = 3.0f;

    [SerializeField]
    [Range(MinRotationSmoothing, MaxRotationSmoothing)]
    private float rotationSmoothing = 15.0f;

    private Transform rig;
    private Transform pivot; // The pivot the camera is rotating around
    private float lookAngle;
    private float tiltAngle;
    private Vector3 initialPivotEulers;
    private Quaternion pivotTargetLocalRotation; // Controls the tilt rotation
    private Quaternion rigTargetLocalRotation; // Controlls the look rotation
    private Vector3 cameraVelocity; // The velocity at which the camera moves

    protected virtual void Awake()
    {
        if (this.autoFindTarget)
        {
            this.target = GameObject.FindGameObjectWithTag(this.targetTag).transform;
        }

        this.pivot = this.transform.parent;
        this.initialPivotEulers = this.pivot.rotation.eulerAngles;

        this.rig = this.pivot.parent;

        // Position the camera
        Vector3 cameraTargetLocalPosition = this.transform.localPosition;
        cameraTargetLocalPosition.z = -this.distanceToTarget;

        this.transform.localPosition = cameraTargetLocalPosition;
    }

    protected virtual void Update()
    {
        this.HandleRotationMovement(Time.deltaTime);
    }

    protected virtual void LateUpdate()
    {
        this.FollowTarget();
    }

    private void FollowTarget()
    {
        if (this.target == null)
        {
            return;
        }

        this.rig.position = Vector3.SmoothDamp(this.rig.position, this.target.position, ref this.cameraVelocity, this.catchSpeedDamp);
    }

    private void HandleRotationMovement(float deltaTime)
    {
        //if (!Input.GetMouseButton(1))
        //{
        //    return;
        //}

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Adjust the look angle
        this.lookAngle += mouseX * this.mouseSensitivity;
        this.rigTargetLocalRotation = Quaternion.Euler(0.0f, this.lookAngle, 0.0f);

        // Adjust the tilt angle
        this.tiltAngle += mouseY * this.mouseSensitivity;
        this.tiltAngle = MathfUtils.ClampAngle(this.tiltAngle, MinTiltAngle, MaxTiltAngle);
        this.pivotTargetLocalRotation = Quaternion.Euler(-this.tiltAngle, this.initialPivotEulers.y, this.initialPivotEulers.z);

        // Rotate the camera
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
