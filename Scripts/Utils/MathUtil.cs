using UnityEngine;

namespace Game.Scripts.Utils
{
    public static class MathUtil
    {
        public static float Remap(float iMin, float iMax, float oMin, float oMax, float value)
        {
            float t = Mathf.InverseLerp(iMin, iMax, value);
            return Mathf.Lerp(oMin, oMax, t);
        }
        
        public static Vector3 Remap(float iMin, float iMax, Vector3 oMin, Vector3 oMax, float value)
        {
            float t = Mathf.InverseLerp(iMin, iMax, value);
            return Vector3.Lerp(oMin, oMax, t);
        }
    }
}