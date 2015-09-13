using System;
using UnityEngine;

[Serializable]
public class GravitySettings
{
    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float gravityStrength = 27f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float maxFallSpeed = 38f;

    public float GravityStrength
    {
        get
        {
            return this.gravityStrength;
        }
        set
        {
            this.gravityStrength = value;
        }
    }

    public float MaxFallSpeed
    {
        get
        {
            return this.maxFallSpeed;
        }
        set
        {
            this.maxFallSpeed = value;
        }
    }
}
