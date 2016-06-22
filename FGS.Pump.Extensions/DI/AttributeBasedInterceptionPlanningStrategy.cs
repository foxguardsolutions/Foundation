using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Ninject.Components;
using Ninject.Extensions.Interception;
using Ninject.Extensions.Interception.Advice;
using Ninject.Extensions.Interception.Planning.Directives;
using Ninject.Extensions.Interception.Registry;
using Ninject.Extensions.Interception.Request;
using Ninject.Planning;
using Ninject.Planning.Strategies;

namespace FGS.Pump.Extensions.DI
{
    /// <remarks>Taken and modified from: http://stackoverflow.com/a/6391216 </remarks>
    public abstract class AttributeBasedInterceptionPlanningStrategyBase<TAttribute> : NinjectComponent, IPlanningStrategy
            where TAttribute : Attribute
    {
        private readonly IAdviceFactory _adviceFactory;
        private readonly IAdviceRegistry _adviceRegistry;

        protected AttributeBasedInterceptionPlanningStrategyBase(IAdviceFactory adviceFactory, IAdviceRegistry adviceRegistry)
        {
            _adviceFactory = adviceFactory;
            _adviceRegistry = adviceRegistry;
        }

        public void Execute(IPlan plan)
        {
            var methods = GetCandidateMethods(plan.Type);
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(TAttribute), true) as TAttribute[];
                if (attributes.Length == 0)
                {
                    continue;
                }

                var advice = _adviceFactory.Create(method);

                advice.Callback = GetInterceptor;
                _adviceRegistry.Register(advice);

                if (!plan.Has<ProxyDirective>())
                {
                    plan.Add(new ProxyDirective());
                }
            }
        }

        protected abstract IInterceptor GetInterceptor(IProxyRequest request);

        private static IEnumerable<MethodInfo> GetCandidateMethods(Type type)
        {
            var methods = type.GetMethods(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            return methods.Where(ShouldIntercept);
        }

        private static bool ShouldIntercept(MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType != typeof(object) &&
                   !methodInfo.IsPrivate &&
                   !methodInfo.IsFinal;
        }
    }
}
