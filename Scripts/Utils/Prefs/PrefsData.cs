using System;

namespace Game.Scripts.Utils.Prefs
{
    public abstract class PrefsData<T> : IDisposable
    {
        private T _value;

        public event Action<T> Changed;

        protected string Key { get; set; }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                Changed?.Invoke(_value);
                Save();
            }
        }

        public virtual void Load(string key) => Key = key;

        public abstract void Save();

        public void Dispose() => Changed = null;
    }
}