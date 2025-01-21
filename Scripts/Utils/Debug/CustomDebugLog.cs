namespace Game.Scripts.Utils.Debug
{
    public class CustomDebugLog
    {
        public static void Log(object message, DebugColor color = DebugColor.Black)
        {
           if (UnityEngine.Debug.isDebugBuild)
                UnityEngine.Debug.Log($"<color=#{ColorDictionary.Colors[color]}>{message}</color>");
        }

        public static void LogWarning(string message, DebugColor color = DebugColor.Black)
        {
            if (UnityEngine.Debug.isDebugBuild)
                UnityEngine.Debug.LogWarning($"<color=#{ColorDictionary.Colors[color]}>{message}</color>");
        }

        public static void LogError(string message, DebugColor color = DebugColor.Black)
        {
            if (UnityEngine.Debug.isDebugBuild)
                UnityEngine.Debug.LogError($"<color=#{ColorDictionary.Colors[color]}>{message}</color>");
        }

        public static void LogException(System.Exception e)
        {
            if (UnityEngine.Debug.isDebugBuild)
                UnityEngine.Debug.LogException(e);
        }
    }
}