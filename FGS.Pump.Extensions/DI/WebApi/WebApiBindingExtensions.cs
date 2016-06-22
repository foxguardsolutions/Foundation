using System;
using System.Web.Http.Filters;

namespace FGS.Pump.Extensions.DI.WebApi
{
    public static class WebApiBindingExtensions
    {
        /// <remarks>Based on the non-filter version from: https://github.com/ninject/Ninject.Extensions.NamedScope/blob/3957ea5e2e5bf100163ca8e5abd9db6084d33701/src/Ninject.Extensions.NamedScope/NamedScopeExtensionMethods.cs </remarks>
        public static Ninject.Web.WebApi.FilterBindingSyntax.IFilterBindingNamedWithOrOnSyntax<T> InParentScope<T>(this Ninject.Web.WebApi.FilterBindingSyntax.IFilterBindingInSyntax<T> self)
            where T : IFilter
        {
            return self.InScope(
                context =>
                    {
                        const string ScopeParameterName = BindingExtensions.InParentScopeScopeParameterName;
                        var parentContext = context.Request.ParentContext;
                        return BindingExtensions.GetOrAddNamedScope(parentContext, ScopeParameterName);
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
    }
}