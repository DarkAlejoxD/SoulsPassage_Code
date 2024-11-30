using System;
using System.Reflection;

namespace UtilsComplements
{
    public static class Singleton
    {
        public static T GetSingleton<T>() where T : class, ISingleton<T>
        {
            return ISingleton<T>.GetInstance();
        }

        public static bool TryGetInstance<T>(out T instance) where T : class, ISingleton<T>
        {
            return ISingleton<T>.TryGetInstance(out instance);
        }

        public static T GetSingleton<T>(this T obj) where T : class, ISingleton<T>
        {
            return obj.GetSingleton<T>();
        }

        public static bool TryGetInstance<T>(this T obj, out T instance) where T : class, ISingleton<T>
        {
            return obj.TryGetInstance(out instance);
        }
    }
}