using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private const float MinTiltAngle = -89.0f;
    private const float MaxTiltAngle = 89.0f;

    [SerializeField]
    private Transform target = null; // The target to follow

    [SerializeField]
    private bool autoFindTarget = true;

    [SerializeField]
    private string targetTag = Tag.Player;

    [SerializeField]
    private float distanceToTarget = 3.0f; // In meters

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float catchSpeedDamp = 0.1f;

    [SerializeField]
    [Range(1.0f, 5.0f)]
    private float mouseSensitivity = 3.0f;

    [SerializeField]
    private float rotationSmooting = 15.0f;

    private Transform rig;
    private Transform pivot; // The pivot the camera is rotating around
    private float lookAngle;
    private float tiltAngle;
    private Vector3 initialCameraLocalPosition;
    private Vector3 initialPivotEulers;
    private Quaternion pivotTargetLocalRotation; // Controls the tilt rotation
    private Quaternion rigTargetLocalRotation; // Controlls the look rotation
    private Vector3 moveVelocity; // The velocity at which the camera moved

    protected virtual void Awake()
    {
        Application.targetFrameRate = 60;

        if (this.autoFindTarget)
        {
            this.target = GameObject.FindGameObjectWithTag(this.targetTag).transform;
        }

        this.initialCameraLocalPosition = this.transform.localPosition;

        this.pivot = this.transform.parent;
        this.initialPivotEulers = this.pivot.rotation.eulerAngles;

        this.rig = this.pivot.parent;
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

        this.rig.position = Vector3.SmoothDamp(this.rig.position, this.target.position, ref this.moveVelocity, this.catchSpeedDamp);

        this.initialCameraLocalPosition.z = -this.distanceToTarget;
        this.transform.localPosition = this.initialCameraLocalPosition;
    }

    private void HandleRotationMovement(float deltaTime)
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Adjust the look angle
        this.lookAngle += mouseX * this.mouseSensitivity;
        this.rigTargetLocalRotation = Quaternion.Euler(0.0f, this.lookAngle, 0.0f);

        // Adjust the tilt angle
        this.tiltAngle += mouseY * this.mouseSensitivity;
        this.tiltAngle = Mathf.Clamp(this.tiltAngle, MinTiltAngle, MaxTiltAngle);
        this.pivotTargetLocalRotation = Quaternion.Euler(-this.tiltAngle, this.initialPivotEulers.y, this.initialPivotEulers.z);

        // Rotate the camera
        if (this.rotationSmooting > 0.0f)
        {
            this.pivot.localRotation = Quaternion.Slerp(this.pivot.localRotation, this.pivotTargetLocalRotation, this.rotationSmooting * deltaTime);
            this.rig.localRotation = Quaternion.Slerp(this.rig.localRotation, this.rigTargetLocalRotation, this.rotationSmooting * deltaTime);
        }
        else
        {
            this.pivot.localRotation = this.pivotTargetLocalRotation;
            this.rig.localRotation = this.rigTargetLocalRotation;
        }
    }
}
