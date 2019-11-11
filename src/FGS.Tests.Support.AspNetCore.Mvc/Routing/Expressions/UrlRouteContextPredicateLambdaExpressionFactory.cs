using System;
using System.Linq;
using System.Linq.Expressions;

using FGS.Linq.Expressions;

using Microsoft.AspNetCore.Mvc.Routing;

namespace FGS.Tests.Support.AspNetCore.Mvc.Routing.Expressions
{
    internal static class UrlRouteContextPredicateLambdaExpressionFactory
    {
        internal static Expression<Func<UrlRouteContext, bool>> Create<TRouteValues>(string routeName, TRouteValues routeValues) =>
            Create(routeName, (object rv) => RouteValuesLooseEqualityComparer.Instance.Equals(routeValues, rv));

        internal static Expression<Func<UrlRouteContext, bool>> Create<TRouteValues>(string routeName, Expression<Func<TRouteValues, bool>> routeValuesPredicateExpression)
        {
            var urlRouteContextParameterExpression = Expression.Parameter(typeof(UrlRouteContext), "urc");
            var routeNameEqualExpression = CreateBody(routeName, urlRouteContextParameterExpression);
            var routeValuesEqualExpression = CreateBody(routeValuesPredicateExpression, urlRouteContextParameterExpression);
            var urlRouteContextPredicateExpression = Expression.And(routeNameEqualExpression, routeValuesEqualExpression);
            return Expression.Lambda<Func<UrlRouteContext, bool>>(urlRouteContextPredicateExpression, urlRouteContextParameterExpression);
        }

        internal static Expression<Func<UrlRouteContext, bool>> Create(string routeName) =>
            Create(routeName, Expression.Parameter(typeof(UrlRouteContext), "urc"));

        private static Expression<Func<UrlRouteContext, bool>> Create(string routeName, ParameterExpression urlRouteContextParameterExpression)
        {
            var equalExpression = CreateBody(routeName, urlRouteContextParameterExpression);
            return Expression.Lambda<Func<UrlRouteContext, bool>>(equalExpression, urlRouteContextParameterExpression);
        }

        private static Expression CreateBody(string routeName, ParameterExpression urlRouteContextParameterExpression)
        {
            return Expression.Equal(
                Expression.PropertyOrField(urlRouteContextParameterExpression, nameof(UrlRouteContext.RouteName)),
                Expression.Constant(routeName));
        }

        private static Expression CreateBody<TRouteValues>(Expression<Func<TRouteValues, bool>> routeValuesPredicateExpression, ParameterExpression urlRouteContextParameterExpression) =>
            ParameterReplacer.Replace<Func<TRouteValues, bool>, Func<bool>>(routeValuesPredicateExpression, routeValuesPredicateExpression.Parameters.Single(), Expression.PropertyOrField(urlRouteContextParameterExpression, nameof(UrlRouteContext.Values))).Body;
    }
}
