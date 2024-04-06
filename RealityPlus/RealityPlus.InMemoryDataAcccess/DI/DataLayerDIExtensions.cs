using Microsoft.Extensions.DependencyInjection;
using RealityPlus.DataModels.Interfaces;
using RealityPlus.InMemoryDataAccess.DataAccess;

namespace RealityPlus.InMemoryDataAccess.DI
{
    public static class DataLayerDIExtensions
    {
        /// <summary>
        ///     Added the Queue manager to the available injectable dependencies
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            services.AddSingleton<IPlayerData, Player>();

            return services;
        }
    }
}
