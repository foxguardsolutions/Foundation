using System;
using System.Threading.Tasks;

using FGS.Primitives.Time.Abstractions;

namespace FGS.Pump.Extensions.DI.Interception
{
    public class FreezeTimeInterceptor : IInterceptor
    {
        private readonly Func<IFreezableClock> _freezableClockFactory;

        public FreezeTimeInterceptor(Func<IFreezableClock> freezableClockFactory)
        {
            _freezableClockFactory = freezableClockFactory;
        }

        public void Intercept(IInvocation invocation)
        {
            var freezableClock = _freezableClockFactory();
            freezableClock.FreezeTime();
            invocation.Proceed();
            freezableClock.UnfreezeTime();
        }

        public async Task InterceptAsync(IAsyncInvocation invocation)
        {
            var freezableClock = _freezableClockFactory();
            freezableClock.FreezeTime();
            await invocation.ProceedAsync();
            freezableClock.UnfreezeTime();
        }
    }
}
