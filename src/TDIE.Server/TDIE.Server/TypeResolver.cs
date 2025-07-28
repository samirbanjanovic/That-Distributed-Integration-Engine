using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using TDIE.Server.Classes;

namespace TDIE.Server
{
    internal static class TypeResolver
    {
        public static T GetConfiguredPlatofrmPieceType<T>(string assemblyPath, string qualifiedClassName)
        {            
            var platformPieceTypeAssembly = Assembly.LoadFrom(assemblyPath);

            Type platformPieceType = platformPieceTypeAssembly.GetType(qualifiedClassName);
            Func<object> instanceBuilder = InstanceInitializer(platformPieceType);

            return (T)instanceBuilder();
        }

        public static Func<object> InstanceInitializer(Type type)
        {
            var instanceExpression = Expression.Lambda<Func<object>>(Expression.New(type
                                                               .GetConstructor(Type.EmptyTypes)));

            return instanceExpression.Compile();
        }
    }
}
