using UnityEngine;

namespace Game.Scripts.Utils.Prefs
{
    public class PrefsInt : PrefsData<int>
    {
        public override void Load(string key)
        {
            base.Load(key);

            Value = PlayerPrefs.GetInt(key, 0);
        }

        public override void Save()
        {
            PlayerPrefs.SetInt(Key, Value);
        }
    }
}