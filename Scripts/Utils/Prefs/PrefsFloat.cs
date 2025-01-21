using UnityEngine;

namespace Game.Scripts.Utils.Prefs
{
    public class PrefsFloat : PrefsData<float>
    {
        public override void Load(string key)
        {
            base.Load(key);

            Value = PlayerPrefs.GetFloat(key, 0);
        }

        public override void Save()
        {
            PlayerPrefs.SetFloat(Key, Value);
        }
    }
}