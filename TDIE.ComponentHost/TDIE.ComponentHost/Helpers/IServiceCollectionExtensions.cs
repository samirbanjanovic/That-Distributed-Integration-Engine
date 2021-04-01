using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace TDIE.ComponentHost.Helpers
{
    internal static class IServiceCollectionExtensions
    {

        public static IServiceCollection RemoveSerivce(this IServiceCollection services, Type type)
        {
            var serviceDescriptor = services.FirstOrDefault(x => x.ServiceType == type);
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }

            return services;
        }

        public static IServiceCollection AddOrReplaceSingleton(this IServiceCollection services, Type type)
        {
            services.RemoveSerivce(type);
            services.AddSingleton(type);

            return services;
        }

        //public static IServiceCollection AddSingleton(this IServiceCollection services, Type interfaceType, Type concreteType)
        //{
        //    var x = (IServiceCollection)typeof(IServiceCollection).GetMethod("Singleton").MakeGenericMethod(interfaceType, concreteType);

        //    return null;
        //}
    }
}
