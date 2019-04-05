using System;

using Castle.DynamicProxy;

namespace FGS.Pump.Extensions.DI.Interception
{
    public class FreezeTimeAsyncInterceptor : NonRacingAsyncInterceptor
    {
        private readonly Func<IFreezableClock> _freezableClockFactory;

        public FreezeTimeAsyncInterceptor(Func<IFreezableClock> freezableClockFactory)
        {
            _freezableClockFactory = freezableClockFactory;
        }

        protected override void BeforeInvoke(IInvocation invocation)
        {
            var freezableClock = _freezableClockFactory();
            freezableClock.FreezeTime();
        }

        protected override void AfterInvoke(IInvocation invocation)
        {
            var freezableClock = _freezableClockFactory();
            freezableClock.UnfreezeTime();
        }
    }
}