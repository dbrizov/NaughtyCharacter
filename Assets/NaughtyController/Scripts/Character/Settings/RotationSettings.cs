using System;
using UnityEngine;

[Serializable]
public class RotationSettings
{
    [SerializeField]
    [Tooltip("How fast the character rotates around the Y axis. Value of 0 disables Rotation Smoothing")]
    private float rotationSmoothing = 15f;

    [SerializeField]
    [Tooltip("Should the character be oriented his rotation to movement? The character can't orient it's rotation to movement and use control rotation at the same time.")]
    private bool orientRotationToMovement = true;

    [SerializeField]
    [Tooltip("Should the character use control rotation? The character can't use control rotation and orient it's rotation to movement at the same time.")]
    private bool useControlRotation = false;

    public float RotationSmoothing
    {
        get
        {
            return this.rotationSmoothing;
        }
        set
        {
            this.rotationSmoothing = value;
        }
    }

    /// <summary>
    /// If set to true, this automatically sets UseControlRotation to false
    /// </summary>
    public bool OrientRotationToMovement
    {
        get
        {
            return this.orientRotationToMovement;
        }
        set
        {
            this.orientRotationToMovement = value;
            if (this.orientRotationToMovement)
            {
                this.useControlRotation = false;
            }
        }
    }

    /// <summary>
    /// If set to true, this automatically sets OrientRotationToMovement to false
    /// </summary>
    public bool UseControlRotation
    {
        get
        {
            return this.useControlRotation;
        }
        set
        {
            this.useControlRotation = value;
            if (this.useControlRotation)
            {
                this.orientRotationToMovement = false;
            }
        }
    }
}
