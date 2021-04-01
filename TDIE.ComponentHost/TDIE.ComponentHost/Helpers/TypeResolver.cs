using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TDIE.ComponentHost.Helpers
{
    internal static class TypeResolver
    {
        public static bool TryGetTypeList(Type implementedTypeToSearchFor, FileInfo fileInfo, out Assembly typeAssembly, out Type[] concreteTypes)
        {
            typeAssembly = null;
            concreteTypes = null;
            bool hasComponent = false;

            typeAssembly = Assembly.LoadFrom(fileInfo.FullName);
            concreteTypes = typeAssembly.GetTypes()
                                        .Where(t => implementedTypeToSearchFor.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                        .ToArray();

            hasComponent = concreteTypes.Any();

            if (!hasComponent)
            {
                typeAssembly = null;
                concreteTypes = null;
            }

            return hasComponent;
        }

        public static bool TryGetType(Type implementedTypeToSearchFor, FileInfo fileInfo, out Assembly typeAssembly, out Type concreteType)
        {
            typeAssembly = null;
            concreteType = null;
            bool hasComponent = false;
            if(hasComponent = TryGetTypeList(implementedTypeToSearchFor, fileInfo, out typeAssembly, out Type[] concreteTypes))
            {
                concreteType = concreteTypes[0];
            }

            return hasComponent;
        }
    }
}
