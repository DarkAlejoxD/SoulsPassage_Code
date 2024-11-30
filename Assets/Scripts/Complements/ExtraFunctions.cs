using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UtilsComplements
{
    public static class ExtraFunctions
    {
        public static IEnumerable GetClasses(Type baseType)
        {
            return Assembly.GetAssembly(baseType).GetTypes().Where
                   (t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));
        }

        /// <summary>
        /// External implementation. Gets all the type that implements a generic interface
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetGenericInterfaceImplementations(Type type)
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces()
                           .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type));
        }
    }
}