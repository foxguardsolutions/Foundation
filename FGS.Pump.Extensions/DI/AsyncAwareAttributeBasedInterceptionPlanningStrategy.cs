using System;
using System.Threading.Tasks;

using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Interception;
using Ninject.Extensions.Interception.Advice;
using Ninject.Extensions.Interception.Registry;
using Ninject.Extensions.Interception.Request;

namespace FGS.Pump.Extensions.DI
{
    public class AsyncAwareAttributeBasedInterceptionPlanningStrategy<TAttribute, TSyncInterceptor, TAsyncInterceptor>
        : AttributeBasedInterceptionPlanningStrategyBase<TAttribute>
        where TAttribute : Attribute
        where TSyncInterceptor : IInterceptor
        where TAsyncInterceptor : NonRacingAsyncInterceptor
    {
        public AsyncAwareAttributeBasedInterceptionPlanningStrategy(IAdviceFactory adviceFactory, IAdviceRegistry adviceRegistry)
            : base(adviceFactory, adviceRegistry)
        {
        }

        #region Overrides of AttributeBasedInterceptionPlanningStrategyBase<TAttribute>

        protected override IInterceptor GetInterceptor(IProxyRequest request)
        {
            if (typeof(Task).IsAssignableFrom(request.Method.ReturnType))
                return request.Context.ContextPreservingGet<TAsyncInterceptor>();
            else
                return request.Context.ContextPreservingGet<TSyncInterceptor>();
        }

        #endregion
    }
}