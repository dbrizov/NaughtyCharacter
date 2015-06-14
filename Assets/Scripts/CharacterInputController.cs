using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]
public class CharacterInputController : MonoBehaviour
{
    public const float MinTiltAngle = -89.0f;
    public const float MaxTiltAngle = 89.0f;
    public const float MinMouseSensitivity = 1f;
    public const float MaxMouseSensitivity = 5f;

    [SerializeField]
    [Tooltip("The default camera is the main camera")]
    private Transform followCamera;

    [SerializeField]
    [Range(MinMouseSensitivity, MaxMouseSensitivity)]
    private float mouseSensitivity = 3.0f;

    private Character character;
    private Vector3 moveVector;
    private float lookAngle;
    private float tiltAngle;
    private Quaternion controlRotation;
    private Quaternion controlRotationX;
    private Quaternion controlRotationY;

    protected virtual void Awake()
    {
        if (this.followCamera == null)
        {
            this.followCamera = Camera.main.transform;
        }

        this.character = this.GetComponent<Character>();
        this.moveVector = Vector3.zero;
        this.lookAngle = 0f;
        this.tiltAngle = 0f;
    }

    protected virtual void Update()
    {
        this.UpdateMoveVector();
        this.UpdateSprintState();
        this.UpdateJumpState();
        this.UpdateControlRotation();

        this.character.Move(this.moveVector);
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

    public Quaternion ControlRotationX
    {
        get
        {
            return this.controlRotationX;
        }
        private set
        {
            this.controlRotationX = value;
        }
    }

    public Quaternion ControlRotationY
    {
        get
        {
            return this.controlRotationY;
        }
        private set
        {
            this.controlRotationY = value;
        }
    }

    private void UpdateMoveVector()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (this.followCamera != null)
        {
            // Calculate the move vector relative to camera direction
            Vector3 scalerVector = new Vector3(1f, 0f, 1f);
            Vector3 cameraForward = Vector3.Scale(this.followCamera.forward, scalerVector).normalized;
            Vector3 cameraRight = Vector3.Scale(this.followCamera.right, scalerVector).normalized;

            this.moveVector = (cameraForward * vertical + cameraRight * horizontal);
        }
        else
        {
            // Use world relative directions
            this.moveVector = (Vector3.forward * vertical + Vector3.right * horizontal);
        }

        if (this.moveVector.magnitude > 1.0f)
        {
            this.moveVector.Normalize();
        }
    }

    private void UpdateControlRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Adjust the look angle (Y Rotation)
        this.lookAngle += mouseX * this.mouseSensitivity;
        this.ControlRotationY = Quaternion.Euler(0f, this.lookAngle, 0f);

        // Adjust the tilt angle (X Rotation)
        this.tiltAngle += mouseY * this.mouseSensitivity;
        this.tiltAngle = MathfExtensions.ClampAngle(this.tiltAngle, MinTiltAngle, MaxTiltAngle);
        this.ControlRotationX = Quaternion.Euler(-this.tiltAngle, 0f, 0f);

        // The entire Control Rotation
        this.ControlRotation = Quaternion.Euler(-this.tiltAngle, this.lookAngle, 0f);

        this.character.ControlRotation = this.ControlRotation;
        this.character.ControlRotationX = this.ControlRotationX;
        this.character.ControlRotationY = this.ControlRotationY;
    }

    private void UpdateSprintState()
    {
        float sprintAxis = Input.GetAxis("Sprint");

        if (sprintAxis > 0f)
        {
            this.character.IsSprinting = true;
        }
        else
        {
            this.character.IsJogging = true;
        }
    }

    private void UpdateJumpState()
    {
        this.character.IsJumping = Input.GetButtonDown("Jump");
    }
}
