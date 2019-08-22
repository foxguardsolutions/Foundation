using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FGS.Extensions.Diagnostics.HealthChecks.EntityFramework
{
    public class EntityFrameworkHealthCheck<TDbContext> : IHealthCheck
        where TDbContext : DbContext
    {
        private readonly Lazy<TDbContext> _lazyDbContext;
        private readonly Func<TDbContext, IQueryable<bool>> _healthQuery;

        public EntityFrameworkHealthCheck(Lazy<TDbContext> lazyDbContext, Func<TDbContext, IQueryable<bool>> healthQuery)
        {
            _lazyDbContext = lazyDbContext;
            _healthQuery = healthQuery;
        }

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
