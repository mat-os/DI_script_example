using UnityEngine;

namespace Game.Scripts.Utils.Extensions
{
    public static class TransformExtensions
    {
        #region Position

        public static void SetPositionX(this Transform transform, float positionX)
        {
            Vector3 position = transform.position;
            position.x = positionX;
            transform.position = position;
        }

        public static void SetPositionY(this Transform transform, float positionY)
        {
            Vector3 position = transform.position;
            position.y = positionY;
            transform.position = position;
        }

        public static void SetPositionZ(this Transform transform, float positionZ)
        {
            Vector3 position = transform.position;
            position.z = positionZ;
            transform.position = position;
        }

        public static void SetLocalPositionX(this Transform transform, float positionX)
        {
            Vector3 position = transform.localPosition;
            position.x = positionX;
            transform.localPosition = position;
        }

        public static void SetLocalPositionY(this Transform transform, float positionY)
        {
            Vector3 position = transform.localPosition;
            position.y = positionY;
            transform.localPosition = position;
        }

        public static void SetLocalPositionZ(this Transform transform, float positionZ)
        {
            Vector3 position = transform.localPosition;
            position.z = positionZ;
            transform.localPosition = position;
        }

        #endregion

        #region Scale

        public static void SetScaleX(this Transform transform, float scaleX)
        {
            Vector3 scale = transform.localScale;
            scale.x = scaleX;
            transform.localScale = scale;
        }

        public static void SetScaleY(this Transform transform, float scaleY)
        {
            Vector3 scale = transform.localScale;
            scale.y = scaleY;
            transform.localScale = scale;
        }

        public static void SetScaleZ(this Transform transform, float scaleZ)
        {
            Vector3 scale = transform.localScale;
            scale.z = scaleZ;
            transform.localScale = scale;
        }

        #endregion

        #region Rotation

        public static void SetEulerAnglesX(this Transform transform, float eulerAnglesX)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = eulerAnglesX;
            transform.eulerAngles = eulerAngles;
        }

        public static void SetEulerAnglesY(this Transform transform, float eulerAnglesY)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.y = eulerAnglesY;
            transform.eulerAngles = eulerAngles;
        }

        public static void SetEulerAnglesZ(this Transform transform, float eulerAnglesZ)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.z = eulerAnglesZ;
            transform.eulerAngles = eulerAngles;
        }

        public static void SetLocalEulerAnglesX(this Transform transform, float eulerAnglesX)
        {
            Vector3 eulerAngles = transform.localEulerAngles;
            eulerAngles.x = eulerAnglesX;
            transform.localEulerAngles = eulerAngles;
        }

        public static void SetLocalEulerAnglesY(this Transform transform, float eulerAnglesY)
        {
            Vector3 eulerAngles = transform.localEulerAngles;
            eulerAngles.y = eulerAnglesY;
            transform.localEulerAngles = eulerAngles;
        }

        public static void SetLocalEulerAnglesZ(this Transform transform, float eulerAnglesZ)
        {
            Vector3 eulerAngles = transform.localEulerAngles;
            eulerAngles.z = eulerAnglesZ;
            transform.localEulerAngles = eulerAngles;
        }

        #endregion

        public static void SetParentAndResetTransform(this Transform transform, Transform parent)
        {
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}