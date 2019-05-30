using System;
using System.Linq.Expressions;

using FGS.Tests.Support.AspNetCore.Mvc.Routing.Expressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using Moq;
using Moq.Language.Flow;

namespace FGS.Tests.Support.AspNetCore.Mvc.Routing
{
    public static class MockUrlHelperExtensions
    {
        public static ISetup<IUrlHelper, string> SetupRouteUrl(this Mock<IUrlHelper> mockUrlHelper, string routeName)
        {
            var urlRouteContextPredicateExpression = UrlRouteContextPredicateLambdaExpressionFactory.Create(routeName);

            return mockUrlHelper.SetupRouteUrl(urlRouteContextPredicateExpression);
        }

        public static ISetup<IUrlHelper, string> SetupRouteUrl<TRouteValues>(this Mock<IUrlHelper> mockUrlHelper, string routeName, TRouteValues routeValues)
        {
            var urlRouteContextPredicateExpression = UrlRouteContextPredicateLambdaExpressionFactory.Create(routeName, routeValues);

            return mockUrlHelper.SetupRouteUrl(urlRouteContextPredicateExpression);
        }

        public static void VerifyRouteUrl(this Mock<IUrlHelper> mockUrlHelper, string routeName, Times times)
        {
            var urlRouteContextPredicateExpression = UrlRouteContextPredicateLambdaExpressionFactory.Create(routeName);

            mockUrlHelper.VerifyRouteUrl(urlRouteContextPredicateExpression, times);
        }

        public static void VerifyRouteUrl<TRouteValues>(this Mock<IUrlHelper> mockUrlHelper, string routeName, TRouteValues routeValues, Times times)
        {
            var urlRouteContextPredicateExpression = UrlRouteContextPredicateLambdaExpressionFactory.Create<TRouteValues>(routeName, routeValues);

            mockUrlHelper.VerifyRouteUrl(urlRouteContextPredicateExpression, times);
        }

        private static ISetup<IUrlHelper, string> SetupRouteUrl(this Mock<IUrlHelper> mockUrlHelper, Expression<Func<UrlRouteContext, bool>> urlRouteContextPredicateExpression)
        {
            var routeUrlInvocationLambdaExpression = UrlHelperRouteUrlInvocationLambdaExpressionFactory.Create(urlRouteContextPredicateExpression);

            return mockUrlHelper.Setup(routeUrlInvocationLambdaExpression);
        }

        private static void VerifyRouteUrl(this Mock<IUrlHelper> mockUrlHelper, Expression<Func<UrlRouteContext, bool>> urlRouteContextPredicateExpression, Times times)
        {
            var routeUrlInvocationLambdaExpression = UrlHelperRouteUrlInvocationLambdaExpressionFactory.Create(urlRouteContextPredicateExpression);

            mockUrlHelper.Verify(routeUrlInvocationLambdaExpression, times);
        }
    }
}
