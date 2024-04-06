using Microsoft.Extensions.DependencyInjection;
using RealityPlus.BusinessLayer.DI;
using RealityPlus.InMemoryDataAccess.DI;
using RealityPlus.Server.Interfaces;
using RealityPlus.Server.Server;
using System.Reflection;

namespace RealityPlus.Server.DI
{
    internal class DependancyInjectrion
    {
        public static IServiceProvider Provider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<Configuration, Configuration>();
            services.AddDataAccess();
            services.AddBusinessLayer();

            // Add the Azure Message Processors
            var clientTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t =>   t.IsAssignableTo(typeof(IServerController)))
                .ToList();

            foreach (var type in clientTypes)
            {
                foreach (var i in type.GetInterfaces())
                {
                    services.AddSingleton(i, type);
                }
            };

            services.AddSingleton<Listener, Listener>();
            return services.BuildServiceProvider();
        }

        public static T GetRequiredService<T>() where T : notnull
        {
            var provider = Provider();
            return provider.GetRequiredService<T>();
        }
    }
}
