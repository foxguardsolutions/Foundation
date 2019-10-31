using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FGS.Extensions.Diagnostics.HealthChecks.EntityFramework
{
    /// <inheritdoc />
    public sealed class EntityFrameworkHealthCheck<TDbContext> : IHealthCheck
        where TDbContext : DbContext
    {
        private readonly Lazy<TDbContext> _lazyDbContext;
        private readonly Func<TDbContext, IQueryable<bool>> _healthQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkHealthCheck{TDbContext}"/> class.
        /// </summary>
        /// <param name="lazyDbContext">The lazy wrapper around the database context to be checked for health.</param>
        /// <param name="healthQuery">The query to be executed against the database context to be checked for health. Must execute without error and return at least 1 row. The return value is ignored.</param>
        internal EntityFrameworkHealthCheck(Lazy<TDbContext> lazyDbContext, Func<TDbContext, IQueryable<bool>> healthQuery)
        {
            _lazyDbContext = lazyDbContext;
            _healthQuery = healthQuery;
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "For health checks, upstream exception types are unknowable by definition")]
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var dbContext = _lazyDbContext.Value;

                await _healthQuery(dbContext).FirstAsync(cancellationToken).ConfigureAwait(false);

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, null, ex);
            }
        }
    }
}
