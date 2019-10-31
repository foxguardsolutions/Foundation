using System;
using System.Threading.Tasks;

using FGS.Interception.Abstractions;
using FGS.Primitives.Time.Abstractions;

namespace FGS.Interceptors.Time
{
    /// <summary>
    /// An implementation of <see cref="IInterceptor"/> that uses a retrievable <see cref="IFreezableClock"/> to freeze time before an inbound invocation,
    /// and then unfreeze after the invocation has been completed.
    /// </summary>
    public sealed class FreezeTimeInterceptor : IInterceptor
    {
        private readonly Func<IFreezableClock> _freezableClockFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezeTimeInterceptor"/> class.
        /// </summary>
        /// <param name="freezableClockFactory">The factory that creates or retrieves the <see cref="IFreezableClock"/> for which time will be manipulated.</param>
        public FreezeTimeInterceptor(Func<IFreezableClock> freezableClockFactory)
        {
            _freezableClockFactory = freezableClockFactory;
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            var freezableClock = _freezableClockFactory();
            freezableClock.FreezeTime();
            invocation.Proceed();
            freezableClock.UnfreezeTime();
        }

        /// <inheritdoc />
        public async Task InterceptAsync(IAsyncInvocation invocation)
        {
            var freezableClock = _freezableClockFactory();
            freezableClock.FreezeTime();
            await invocation.ProceedAsync();
            freezableClock.UnfreezeTime();
        }
    }
}
