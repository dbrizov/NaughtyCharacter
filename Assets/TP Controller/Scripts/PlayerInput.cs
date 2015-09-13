using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    // Const variables
    private const float MinTiltAngle = -75.0f;
    private const float MaxTiltAngle = 45.0f;
    private const float MinMouseSensitivity = 1f;
    private const float MaxMouseSensitivity = 5f;

    // Serializable fields 
    [SerializeField]
    [Tooltip("The default camera is the main camera")]
    private Transform followCamera;

    [SerializeField]
    [Range(MinMouseSensitivity, MaxMouseSensitivity)]
    private float mouseSensitivity = 3.0f;

    // Private fields
    private float lookAngle;
    private float tiltAngle;

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        if (this.followCamera == null)
        {
            this.followCamera = Camera.main.transform;
        }

        this.lookAngle = 0f;
        this.tiltAngle = 0f;
    }

    public Vector3 MovementInput()
    {
        Vector3 moveVector;
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        if (this.followCamera != null)
        {
            // Calculate the move vector relative to camera rotation
            Vector3 scalerVector = new Vector3(1f, 0f, 1f);
            Vector3 cameraForward = Vector3.Scale(this.followCamera.forward, scalerVector).normalized;
            Vector3 cameraRight = Vector3.Scale(this.followCamera.right, scalerVector).normalized;

            moveVector = (cameraForward * verticalAxis + cameraRight * horizontalAxis);
        }
        else
        {
            // Use world relative directions
            moveVector = (Vector3.forward * verticalAxis + Vector3.right * horizontalAxis);
        }

        if (moveVector.magnitude > 1f)
        {
            moveVector.Normalize();
        }

        return moveVector;
    }

    public Quaternion MouseRotationInput()
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

        var controlRotation = Quaternion.Euler(-this.tiltAngle, this.lookAngle, 0f);
        return controlRotation;
    }

    public bool SprintInput()
    {
        return Input.GetButton("Sprint");
    }

    public bool JumpInput()
    {
        return Input.GetButtonDown("Jump");
    }

    public bool ToggleWalkInput()
    {
        return Input.GetButtonDown("Toggle Walk");
    }
}
