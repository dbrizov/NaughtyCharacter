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
}

