using System;
using System.Linq;
using System.Reflection;
using System.Timers;
using OnTrac.Integration.Core;

namespace OnTrac.Integration.ComponentHost.Helpers
{
    internal static class StaticHelpers
    {
        public static Type GetTypeFromAssembly(string assemblyPath, string qualifiedClassName)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly.GetType(qualifiedClassName);
        }

        public static Timer GetInitializationTimer(int delay, Action<object, ElapsedEventArgs> onElapsed)
        {
            var timer = new Timer(delay);
            timer.Elapsed += (sender, args) => onElapsed(sender, args);
            timer.AutoReset = false;

            return timer;
        }

        public static IComponent CreateComponentInstance(Type componentType, IMessagePublisher messagePublisher)
        {
            return null;
        }
    }
}
