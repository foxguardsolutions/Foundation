using System;
using System.Threading.Tasks;

namespace FGS.Pump.Extensions.Lifetime
{
    /// <remarks>Taken and modified from: https://github.com/foxguardsolutions/psa/blob/4dcebb2ad1152c3c998c800b662198e01c3bd682/CPSA.Core.Contracts/Lifetime/FuncOwnedExtensions.cs </remarks>
    public static class FuncOwnedExtensions
    {
        public static void Use<TService>(this Func<Owned<TService>> ownedServiceFactory, Action<TService> action)
        {
            using (var ownedService = ownedServiceFactory())
            {
                action(ownedService.Value);
            }
        }

        public static TResult Use<TService, TResult>(this Func<Owned<TService>> ownedServiceFactory, Func<TService, TResult> action)
        {
            using (var ownedService = ownedServiceFactory())
            {
                return action(ownedService.Value);
            }
        }

        public static async Task Use<TService>(this Func<Owned<TService>> ownedServiceFactory, Func<TService, Task> action)
        {
            using (var ownedService = ownedServiceFactory())
            {
                await action(ownedService.Value);
            }
        }

        public static async Task<TResult> Use<TService, TResult>(this Func<Owned<TService>> ownedServiceFactory, Func<TService, Task<TResult>> action)
        {
            using (var ownedService = ownedServiceFactory())
            {
                return await action(ownedService.Value);
            }
        }
    }
}
