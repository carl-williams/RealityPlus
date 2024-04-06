using Microsoft.Extensions.DependencyInjection;
using RealityPlus.BusinessLayer.BusinessLayer;
using RealityPlus.Models.Interfaces;

namespace RealityPlus.BusinessLayer.DI
{
    public static class BusinessLayerDIExtensions
    {
        /// <summary>
        ///     Added the Queue manager to the available injectable dependencies
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            services.AddSingleton<IPasswordManageer, PasswordManager>();
            services.AddSingleton<IPlayer, Player>();
            services.AddSingleton<IGameMatch, GameMatch>();

            return services;
        }
    }
}
