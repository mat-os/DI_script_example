using UnityEngine;

namespace Game.Scripts.Utils.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 SetX(this Vector3 vector, float x)
        {
            vector.x = x;
            return vector;
        }

        public static Vector3 SetY(this Vector3 vector, float y)
        {
            vector.y = y;
            return vector;
        }

        public static Vector3 SetZ(this Vector3 vector, float z)
        {
            vector.z = z;
            return vector;
        }

        public static Vector3 SetXY(this Vector3 vector, float x, float y)
        {
            vector.x = x;
            vector.y = y;
            return vector;
        }

        public static Vector3 SetXZ(this Vector3 vector, float x, float z)
        {
            vector.x = x;
            vector.z = z;
            return vector;
        }

        public static Vector3 SetYZ(this Vector3 vector, float y, float z)
        {
            vector.y = y;
            vector.z = z;
            return vector;
        }

        public static Vector3 AddXY(this Vector3 vector, float x, float y)
        {
            vector.x += x;
            vector.y += y;
            return vector;
        }

        public static Vector3 AddXZ(this Vector3 vector, float x, float z)
        {
            vector.x += x;
            vector.z += z;
            return vector;
        }

        public static Vector3 AddYZ(this Vector3 vector, float y, float z)
        {
            vector.y += y;
            vector.z += z;
            return vector;
        }

        public static Vector2 SetX(this Vector2 vector, float x)
        {
            vector.x = x;
            return vector;
        }

        public static Vector2 SetY(this Vector2 vector, float y)
        {
            vector.y = y;
            return vector;
        }
    }
}