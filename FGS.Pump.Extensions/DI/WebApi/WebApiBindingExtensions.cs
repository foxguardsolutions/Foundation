using System;
using System.Runtime.CompilerServices;
using System.Web.Http.Filters;

using Ninject.Syntax;
using Ninject.Web.WebApi.FilterBindingSyntax;

namespace FGS.Pump.Extensions.DI.WebApi
{
    public static class WebApiBindingExtensions
    {
        /// <remarks>Based on the non-filter version from: https://github.com/ninject/Ninject.Extensions.NamedScope/blob/3957ea5e2e5bf100163ca8e5abd9db6084d33701/src/Ninject.Extensions.NamedScope/NamedScopeExtensionMethods.cs </remarks>
        public static IFilterBindingNamedWithOrOnSyntax<T> InParentScope<T>(this IFilterBindingInSyntax<T> self)
            where T : IFilter
        {
            return self.InScope(
                context =>
                    {
                        const string ScopeParameterName = BindingExtensions.InParentScopeScopeParameterName;
                        var parentContext = context.Request.ParentContext;
                        return BindingExtensions.GetOrAddNamedScope(parentContext ?? context, ScopeParameterName);
                    });
        }

        public static IFilterBindingNamedWithOrOnSyntax<T> In<T>(this IFilterBindingInSyntax<T> self, Scope scope)
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

        public static void BindDecoraptedHttpAuthorizationFilter<T>(this IBindingRoot kernel, FilterScope filterScope, Scope httpScope, Func<IFilterBindingWhenSyntax<IAuthorizationFilter>, IFilterBindingInNamedWithOrOnSyntax<IAuthorizationFilter>> when = null, Func<IFilterBindingWithSyntax<IAuthorizationFilter>, IFilterBindingWithOrOnSyntax<IAuthorizationFilter>> with = null)
            where T : IAuthorizationFilter
        {
            var decoraptorBound = BindingRootExtensions.BindHttpFilter<ApiAuthorizationFilterDecoraptor<T>>(kernel, filterScope);

            var name = "BindHttpFilter" + Guid.NewGuid();

            if (when != null)
            {
                when(decoraptorBound).In(httpScope).Named(name);
            }
            else
            {
                decoraptorBound.In(httpScope).Named(name);
            }

            var actualBound = kernel.Bind<T>().ToSelf().WhenAnyAncestorNamed(name).In(httpScope);

            if (with != null)
            {
                var syntaxAdapter = new DecoraptedApiFilterBindingWithSyntaxAdapter<T, IAuthorizationFilter>(actualBound);
                with(syntaxAdapter);
            }
        }

        public static void BindDecoraptedHttpActionFilter<T>(this IBindingRoot kernel, FilterScope filterScope, Scope httpScope, Func<IFilterBindingWhenSyntax<IActionFilter>, IFilterBindingInNamedWithOrOnSyntax<IActionFilter>> when = null, Func<IFilterBindingWithSyntax<IActionFilter>, IFilterBindingWithOrOnSyntax<IActionFilter>> with = null)
            where T : IActionFilter
        {
            var decoraptorBound = BindingRootExtensions.BindHttpFilter<ApiActionFilterDecoraptor<T>>(kernel, filterScope);

            var name = "BindHttpFilter" + Guid.NewGuid();

            if (when != null)
            {
                when(decoraptorBound).In(httpScope).Named(name);
            }
            else
            {
                decoraptorBound.In(httpScope).Named(name);
            }

            var actualBound = kernel.Bind<T>().ToSelf().WhenAnyAncestorNamed(name).In(httpScope);

            if (with != null)
            {
                var syntaxAdapter = new DecoraptedApiFilterBindingWithSyntaxAdapter<T, IActionFilter>(actualBound);
                with(syntaxAdapter);
            }
        }
    }
}