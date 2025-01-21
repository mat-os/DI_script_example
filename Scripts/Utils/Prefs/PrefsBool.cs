using UnityEngine;

namespace Game.Scripts.Utils.Prefs
{
    public class PrefsBool : PrefsData<bool>
    {
        public PrefsBool(bool defaultValue = false, string key = "")
        {
            Key = key;
            Value = defaultValue;
        }

        public override void Load(string key)
        {
            base.Load(key);
            int value = PlayerPrefs.GetInt(key, 0);
            Value = value == 1;
        }

        public override void Save()
        {
            int value = Value ? 1 : 0;
            PlayerPrefs.SetInt(Key, value); 
        }
    }
}