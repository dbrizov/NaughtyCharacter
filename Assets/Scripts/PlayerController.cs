using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private float horizontalSpeed;
    private float horizontalAxis;
    private float verticalAxis;

    private int horizontalSpeedHashKey = Animator.StringToHash("HorizontalSpeed");
    private int directionHashKey = Animator.StringToHash("Direction");

    public float directionDampTime = 0.25f;

    private void Start()
    {
        this.animator = this.GetComponent<Animator>();
        if (this.animator == null)
        {
            throw new NullReferenceException("The character has no animator component");
        }
    }

    private void Update()
    {
        this.HandleCharacterMovement();
    }

    private void HandleCharacterMovement()
    {
        this.horizontalAxis = Input.GetAxis("Horizontal");
        this.verticalAxis = Input.GetAxis("Vertical");

        this.horizontalSpeed = this.horizontalAxis * this.horizontalAxis + this.verticalAxis;

        this.animator.SetFloat(this.horizontalSpeedHashKey, this.horizontalSpeed);
        this.animator.SetFloat(this.directionHashKey, this.horizontalAxis, this.directionDampTime, Time.deltaTime);
    }
}
