using UnityEngine;
using System.Collections;

public class CharacterInputController : MonoBehaviour
{
    [SerializeField]
    private Transform followCamera;

    private Character character;
    private Vector3 moveVector;

    protected virtual void Awake()
    {
        if (this.followCamera == null)
        {
            this.followCamera = Camera.main.transform;
        }

        this.character = this.GetComponent<Character>();
    }

    protected virtual void Update()
    {
        this.UpdateMoveVector();
        this.UpdateHorizontalSpeed();
    }

    protected virtual void FixedUpdate()
    {
        this.character.Move(this.moveVector);
    }

    private void UpdateMoveVector()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (this.followCamera != null)
        {
            // Calculate the move vector relative to camera direction
            Vector3 cameraForward = Vector3.Scale(this.followCamera.forward, new Vector3(1.0f, 0.0f, 1.0f)).normalized;
            Vector3 cameraRight = Vector3.Scale(this.followCamera.right, new Vector3(1.0f, 0.0f, 1.0f)).normalized;

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

    private void UpdateHorizontalSpeed()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            this.character.IsSprinting = true;
        }

        if (Input.GetButtonUp("Sprint"))
        {
            this.character.IsJogging = true;
        }
    }
}
