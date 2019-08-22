using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FGS.Extensions.Diagnostics.HealthChecks.EntityFramework
{
    public static class HealthChecksBuilderExtensions
    {
        public static void AddEntityFramework<TDbContext>(this IHealthChecksBuilder builder, Func<IServiceProvider, TDbContext> dbContextFactory, Func<TDbContext, IQueryable<bool>> healthQuery, string name = null, HealthStatus? failureStatus = null, IEnumerable<string> tags = null)
            where TDbContext : DbContext
        {
            builder.Add(
                new HealthCheckRegistration(
                    name ?? "entityframework",
                    sp =>
                    {
                        var lazyDbContext = new Lazy<TDbContext>(() => dbContextFactory(sp), LazyThreadSafetyMode.ExecutionAndPublication);
                        return new EntityFrameworkHealthCheck<TDbContext>(lazyDbContext, healthQuery) as IHealthCheck;
                    },
                    failureStatus,
                    tags));
        }

        public static void AddEntityFramework<TDbContext>(this IHealthChecksBuilder builder, Func<IServiceProvider, Lazy<TDbContext>> lazyDbContextFactory, Func<TDbContext, IQueryable<bool>> healthQuery, string name = null, HealthStatus? failureStatus = null, IEnumerable<string> tags = null)
            where TDbContext : DbContext
        {
            builder.Add(
                new HealthCheckRegistration(
                    name ?? "entityframework",
                    sp => new EntityFrameworkHealthCheck<TDbContext>(lazyDbContextFactory(sp), healthQuery) as IHealthCheck,
                    failureStatus,
                    tags));
        }
    }
}
