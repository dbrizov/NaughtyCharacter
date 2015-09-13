using System;
using UnityEngine;

public static class MathfExtensions
{
    public static float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360f || angle > 360f)
        {
            if (angle < -360f)
            {
                angle += 360f;
            }
            else if (angle > 360f)
            {
                angle -= 360f;
            }
        }

        return Mathf.Clamp(angle, min, max);
    }

    public static float ApplyGravity(float speed, float gravity, float maxSpeed)
    {
        return Mathf.Max(-maxSpeed, speed - gravity * Time.deltaTime);
    }

    public static float AccelerateSpeed(float speed, float acceleration, float maxSpeed, bool negative)
    {
        if (negative)
        {
            return Mathf.Max(-maxSpeed, speed - acceleration * Time.deltaTime);
        }

        return Mathf.Min(maxSpeed, speed + acceleration * Time.deltaTime);
    }

    public static float DecelerateSpeed(float speed, float deceleration)
    {
        if (speed > 0f)
        {
            return Mathf.Max(0f, speed - deceleration * Time.deltaTime);
        }

        return Mathf.Min(0f, speed + deceleration * Time.deltaTime);
    }
}

