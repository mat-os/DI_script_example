using UnityEngine;

namespace Game.Scripts.Utils.Prefs
{
    public class PrefsString : PrefsData<string>
    {
        public override void Load(string key)
        {
            base.Load(key);

            Value = PlayerPrefs.GetString(key, string.Empty);
        }

        public override void Save()
        {
            PlayerPrefs.SetString(Key, Value);
        }
    }
}