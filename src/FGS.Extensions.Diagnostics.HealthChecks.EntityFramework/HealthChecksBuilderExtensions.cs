using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FGS.Extensions.Diagnostics.HealthChecks.EntityFramework
{
    /// <summary>
    /// Extends <see cref="IHealthChecksBuilder"/> with the ability to add a <see cref="IHealthCheck"/> that checks an Entity Framework 6 database context.
    /// </summary>
    public static class HealthChecksBuilderExtensions
    {
        /// <summary>
        /// Adds an <see cref="EntityFrameworkHealthCheck{TDbContext}"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/> that is being added to.</param>
        /// <param name="dbContextFactory">A factory that can resolve a <typeparamref name="TDbContext"/> from a <see cref="IServiceProvider"/>.</param>
        /// <param name="healthQuery">The query to be executed against the database context to be checked for health. Must execute without error and return at least 1 row. The return value is ignored.</param>
        /// <param name="name">The name of the health check being registered. If <see langword="null"/>, defaults to <value>entityframework</value>.</param>
        /// <param name="failureStatus">The <see cref="HealthStatus"/> to return if the health check fails. If <see langword="null"/>, defaults to <see cref="HealthStatus.Unhealthy"/>.</param>
        /// <param name="tags">A list of tags that can be used for filtering health checks.</param>
        /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> against which health is being checked.</typeparam>
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

        /// <summary>
        /// Adds an <see cref="EntityFrameworkHealthCheck{TDbContext}"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/> that is being added to.</param>
        /// <param name="lazyDbContextFactory">A lazy wrapper around a factory that can resolve a <typeparamref name="TDbContext"/> from a <see cref="IServiceProvider"/>.</param>
        /// <param name="healthQuery">The query to be executed against the database context to be checked for health. Must execute without error and return at least 1 row. The return value is ignored.</param>
        /// <param name="name">The name of the health check being registered. If <see langword="null"/>, defaults to <value>entityframework</value>.</param>
        /// <param name="failureStatus">The <see cref="HealthStatus"/> to return if the health check fails. If <see langword="null"/>, defaults to <see cref="HealthStatus.Unhealthy"/>.</param>
        /// <param name="tags">A list of tags that can be used for filtering health checks.</param>
        /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> against which health is being checked.</typeparam>
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
