using System;
using System.Linq.Expressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using Moq;

namespace FGS.Tests.Support.AspNetCore.Mvc.Routing.Expressions
{
    internal static class UrlHelperRouteUrlInvocationLambdaExpressionFactory
    {
        internal static Expression<Func<IUrlHelper, string>> Create(Expression<Func<UrlRouteContext, bool>> urlRouteContextPredicateExpression)
        {
            var urlHelperParameter = Expression.Parameter(typeof(IUrlHelper), "uh");

            var urlRouteContextPredicateSentinelExpression = ParameterSentinelExpression(urlRouteContextPredicateExpression);

            var urlHelperRouteUrlExpression = Expression.Call(urlHelperParameter, nameof(IUrlHelper.RouteUrl), null, urlRouteContextPredicateSentinelExpression);

            return Expression.Lambda<Func<IUrlHelper, string>>(urlHelperRouteUrlExpression, urlHelperParameter);
        }

        private static MethodCallExpression ParameterSentinelExpression<TParameter>(Expression<Func<TParameter, bool>> parameterPredicateExpression) =>
            Expression.Call(typeof(It), nameof(It.Is), new[] { typeof(TParameter) }, parameterPredicateExpression);
    }
}
