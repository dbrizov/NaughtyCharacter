using System;
using UnityEngine;

namespace Utils
{
    public static class MathfUtils
    {
        public static float ClampAngle(float angle, float min, float max)
        {
            do
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
            while (angle < -360 || angle > 360);

            return Mathf.Clamp(angle, min, max);
        }
    }
}

