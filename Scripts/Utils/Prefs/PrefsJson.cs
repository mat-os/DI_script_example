using UnityEngine;

namespace Game.Scripts.Utils.Prefs
{
    public class PrefsJson<T> : PrefsData<T>
    {
        public override void Load(string key)
        {
            base.Load(key);
            string json = PlayerPrefs.GetString(key, string.Empty);
            Value = JsonUtility.FromJson<T>(json);
            UnityEngine.Debug.Log($"[Load] Key: {Key}, JSON: {Value}"); // Вывод JSON в консоль
        }

        public override void Save()
        {
            string json = JsonUtility.ToJson(Value);
            PlayerPrefs.SetString(Key, json);
            UnityEngine.Debug.Log($"[Save] Key: {Key}, JSON: {json}"); // Вывод JSON в консоль
        }
    }
}