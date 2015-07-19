using UnityEngine;
using System;
using System.Collections;

public class CharacterInputController : MonoBehaviour
{
    // Delegates and Events
    public delegate void MoveInputHandler(Vector3 moveVector);
    public delegate void MouseRotationInputHandler(Quaternion controlRotation);
    public delegate void JumpInputHandler();
    public delegate void SprintInputHandler(bool isSprinting);

    public static event MoveInputHandler OnMoveInput;
    public static event MouseRotationInputHandler OnMouseRotationInput;
    public static event JumpInputHandler OnJumpInput;
    public static event SprintInputHandler OnSprintInput;

    // Const variables
    public const float MinTiltAngle = -75.0f;
    public const float MaxTiltAngle = 45.0f;
    public const float MinMouseSensitivity = 1f;
    public const float MaxMouseSensitivity = 5f;

    // Serializable fields 
    [SerializeField]
    [Tooltip("The default camera is the main camera")]
    private Transform followCamera;

    [SerializeField]
    [Range(MinMouseSensitivity, MaxMouseSensitivity)]
    private float mouseSensitivity = 3.0f;

    // Private fields
    private Vector3 moveVector;
    private float lookAngle;
    private float tiltAngle;
    private Quaternion controlRotation;

    protected virtual void Awake()
    {
        if (this.followCamera == null)
        {
            this.followCamera = Camera.main.transform;
        }

        this.moveVector = Vector3.zero;
        this.lookAngle = 0f;
        this.tiltAngle = 0f;
    }

    protected virtual void Update()
    {
        this.UpdateSprintState();
        this.UpdateJumpState();
        this.UpdateControlRotation();
        this.UpdateMoveVector();
    }

    public Quaternion ControlRotation
    {
        get
        {
            return this.controlRotation;
        }
        private set
        {
            this.controlRotation = value;
        }
    }

    private void UpdateMoveVector()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        if (this.followCamera != null)
        {
            // Calculate the move vector relative to camera direction
            Vector3 scalerVector = new Vector3(1f, 0f, 1f);
            Vector3 cameraForward = Vector3.Scale(this.followCamera.forward, scalerVector).normalized;
            Vector3 cameraRight = Vector3.Scale(this.followCamera.right, scalerVector).normalized;

            this.moveVector = (cameraForward * verticalAxis + cameraRight * horizontalAxis);
        }
        else
        {
            // Use world relative directions
            this.moveVector = (Vector3.forward * verticalAxis + Vector3.right * horizontalAxis);
        }

        if (this.moveVector.magnitude > 1f)
        {
            this.moveVector.Normalize();
        }

        if (OnMoveInput != null)
        {
            OnMoveInput(this.moveVector);
        }
    }

    private void UpdateControlRotation()
    {
        //if (!Input.GetMouseButton(1))
        //{
        //    return;
        //}

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Adjust the look angle (Y Rotation)
        this.lookAngle += mouseX * this.mouseSensitivity;
        this.lookAngle %= 360f;

        // Adjust the tilt angle (X Rotation)
        this.tiltAngle += mouseY * this.mouseSensitivity;
        this.tiltAngle %= 360f;
        this.tiltAngle = MathfExtensions.ClampAngle(this.tiltAngle, MinTiltAngle, MaxTiltAngle);

        // The entire Control Rotation
        this.ControlRotation = Quaternion.Euler(-this.tiltAngle, this.lookAngle, 0f);

        if (OnMouseRotationInput != null)
        {
            OnMouseRotationInput(this.ControlRotation);
        }
    }

    private void UpdateSprintState()
    {
        bool isSprinting = Input.GetAxis("Sprint") > 0f ? true : false;

        if (OnSprintInput != null)
        {
            OnSprintInput(isSprinting);
        }
    }

    private void UpdateJumpState()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (OnJumpInput != null)
            {
                OnJumpInput();
            }
        }
    }
}
