using System;

namespace Extensions
{
    public static class ReadabilityExtensions
    {
        public static T With<T>(this T self, Action<T> set)
        {
            set.Invoke(self);
            return self;
        }
 
        public static T With<T>(this T self, Action<T> apply, Func<bool> when)
        {
            if (when())
                apply?.Invoke(self);
 
            return self;
        }
 
        public static T With<T>(this T self, Action<T> apply, bool when)
        {
            if (when)
                apply?.Invoke(self);
 
            return self;
        }
    }
}