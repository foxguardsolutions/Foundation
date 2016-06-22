using System;
using System.Linq;
using System.Web.Http.Filters;

using FGS.Pump.Eventing;

using Ninject.Activation;
using Ninject.Extensions.NamedScope;
using Ninject.Syntax;
using Ninject.Web.Common;

namespace FGS.Pump.Extensions.DI
{
    public static class BindingExtensions
    {
        private const string InParentScopeScopeParameterName = "NamedScopeInParentScope";

        public static void BindEventUnhandled<TEvent>(this BindingRoot self)
            where TEvent : Event
        {
            self.Bind<IEventHandler<TEvent>>().To<LoggingNullEventHandler>().InSingletonScope();
        }

        public static void BindEventHandler<TEvent, TEventHandler>(this BindingRoot self, Scope scope)
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : Event
        {
            In(self.Bind<IEventHandler<TEvent>>().To<PreEventHandlerTraceLoggingDecorator<TEvent, TEventHandler>>(), scope);
            In(self.Bind<TEventHandler>().ToSelf().WhenInjectedExactlyInto<PreEventHandlerTraceLoggingDecorator<TEvent, TEventHandler>>(), scope);
        }

        /// <remarks>Based on the non-filter version from: https://github.com/ninject/Ninject.Extensions.NamedScope/blob/3957ea5e2e5bf100163ca8e5abd9db6084d33701/src/Ninject.Extensions.NamedScope/NamedScopeExtensionMethods.cs </remarks>
        public static Ninject.Web.WebApi.FilterBindingSyntax.IFilterBindingNamedWithOrOnSyntax<T> InParentScope<T>(this Ninject.Web.WebApi.FilterBindingSyntax.IFilterBindingInSyntax<T> self)
            where T : IFilter
        {
            return self.InScope(
                context =>
                {
                    const string ScopeParameterName = InParentScopeScopeParameterName;
                    var parentContext = context.Request.ParentContext;
                    return GetOrAddNamedScope(parentContext, ScopeParameterName);
                });
        }

        /// <remarks>Based on the non-filter version from: https://github.com/ninject/Ninject.Extensions.NamedScope/blob/3957ea5e2e5bf100163ca8e5abd9db6084d33701/src/Ninject.Extensions.NamedScope/NamedScopeExtensionMethods.cs </remarks>
        public static Ninject.Web.Mvc.FilterBindingSyntax.IFilterBindingNamedWithOrOnSyntax<T> InParentScope<T>(this Ninject.Web.Mvc.FilterBindingSyntax.IFilterBindingInSyntax<T> self)
        {
            return self.InScope(
                context =>
                {
                    const string ScopeParameterName = InParentScopeScopeParameterName;
                    var parentContext = context.Request.ParentContext;
                    return GetOrAddNamedScope(parentContext, ScopeParameterName);
                });
        }

        public static Ninject.Web.WebApi.FilterBindingSyntax.IFilterBindingNamedWithOrOnSyntax<T> In<T>(this Ninject.Web.WebApi.FilterBindingSyntax.IFilterBindingInSyntax<T> self, Scope scope)
            where T : IFilter
        {
            switch (scope)
            {
                case Scope.Transient:
                    return self.InTransientScope();
                case Scope.Parent:
                    return self.InParentScope();
                case Scope.PerRequest:
                    return self.InRequestScope();
                case Scope.Singleton:
                    return self.InSingletonScope();
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
        }

        public static Ninject.Web.Mvc.FilterBindingSyntax.IFilterBindingNamedWithOrOnSyntax<T> In<T>(this Ninject.Web.Mvc.FilterBindingSyntax.IFilterBindingInSyntax<T> self, Scope scope)
        {
            switch (scope)
            {
                case Scope.Transient:
                    return self.InTransientScope();
                case Scope.Parent:
                    return self.InParentScope();
                case Scope.PerRequest:
                    return self.InRequestScope();
                case Scope.Singleton:
                    return self.InSingletonScope();
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
        }

        public static IBindingNamedWithOrOnSyntax<T> In<T>(this IBindingInSyntax<T> self, Scope scope)
        {
            switch (scope)
            {
                case Scope.Transient:
                    return self.InTransientScope();
                case Scope.Parent:
                    return self.InParentScope();
                case Scope.PerRequest:
                    return self.InRequestScope();
                case Scope.Singleton:
                    return self.InSingletonScope();
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
        }

        /// <summary>
        /// Gets the specified named scope or adds a scope with the specified name in case it does not exist yet.
        /// </summary>
        /// <param name="parentContext">The parent context.</param><param name="scopeParameterName">Name of the scope parameter.</param>
        /// <returns>
        /// The requested scope.
        /// </returns>
        /// <remarks>Taken from: https://github.com/ninject/Ninject.Extensions.NamedScope/blob/3957ea5e2e5bf100163ca8e5abd9db6084d33701/src/Ninject.Extensions.NamedScope/NamedScopeExtensionMethods.cs </remarks>
        private static object GetOrAddNamedScope(IContext parentContext, string scopeParameterName)
        {
            var namedScopeParameter = GetNamedScopeParameter(parentContext, scopeParameterName);
            if (namedScopeParameter == null)
            {
                namedScopeParameter = new NamedScopeParameter(scopeParameterName);
                parentContext.Parameters.Add(namedScopeParameter);
            }

            return namedScopeParameter.Scope;
        }

        /// <summary>
        /// Gets a named scope parameter from a context.
        /// </summary>
        /// <param name="context">The context.</param><param name="scopeParameterName">Name of the scope parameter.</param>
        /// <returns>
        /// The requested parameter of null if it is not found.
        /// </returns>
        /// <remarks>Taken from: https://github.com/ninject/Ninject.Extensions.NamedScope/blob/3957ea5e2e5bf100163ca8e5abd9db6084d33701/src/Ninject.Extensions.NamedScope/NamedScopeExtensionMethods.cs </remarks>
        private static NamedScopeParameter GetNamedScopeParameter(IContext context, string scopeParameterName)
        {
            return context.Parameters.OfType<NamedScopeParameter>().SingleOrDefault(parameter => parameter.Name == scopeParameterName);
        }
    }
}
