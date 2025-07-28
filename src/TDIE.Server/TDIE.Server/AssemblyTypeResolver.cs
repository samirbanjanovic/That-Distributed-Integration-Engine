using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using TDIE.Server.Core;

namespace TDIE.Server
{
    public class AssemblyTypeResolver        
    {

        private readonly ILogger<AssemblyTypeResolver> _logger;

        public AssemblyTypeResolver(ILogger<AssemblyTypeResolver> logger)
        {
            _logger = logger;
        }

        public bool TryGetTypeList(Type implementedTypeToSearchFor, FileInfo fileInfo, out Assembly typeAssembly, out Type[] concreteTypes)
        {
            typeAssembly = null;
            concreteTypes = null;
            bool hasComponent = false;
            try
            {
                typeAssembly = Assembly.LoadFrom(fileInfo.FullName);
                concreteTypes = typeAssembly.GetTypes()
                                            .Where(t => implementedTypeToSearchFor.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                            .ToArray();

                hasComponent = concreteTypes.Any();
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "{Message}", $"Failed to discover types in {fileInfo.FullName}");

                hasComponent = false;
            }

            if(!hasComponent)
            {
                typeAssembly = null;
                concreteTypes = null;
            }


            return hasComponent;
        }

        public bool TryGetType(Type implementedTypeToSearchFor, FileInfo fileInfo, out Assembly typeAssembly, out Type concreteType)
        {
            typeAssembly = null;
            concreteType = null;
            bool hasComponent = false;
            try
            {
                typeAssembly = Assembly.LoadFrom(fileInfo.FullName);
                concreteType = typeAssembly.GetTypes()
                                            .FirstOrDefault(t => implementedTypeToSearchFor.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                hasComponent = !(concreteType is null);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{Message}", $"Failed to discover types in {fileInfo.FullName}");

                hasComponent = false;
            }

            if (!hasComponent)
            {
                typeAssembly = null;
                concreteType = null;
            }

            return hasComponent;
        }
    }
}
