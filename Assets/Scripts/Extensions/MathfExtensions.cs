using System;
using UnityEngine;

public static class MathfExtensions
{
    public static float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360 || angle > 360)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            else if (angle > 360)
            {
                angle -= 360;
            }
        }

        return Mathf.Clamp(angle, min, max);
    }
}

