using System;
using System.Linq.Expressions;

using FGS.Tests.Support.AspNetCore.Mvc.Routing.Expressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using Moq;
using Moq.Language.Flow;

namespace FGS.Tests.Support.AspNetCore.Mvc.Routing
{
    /// <summary>
    /// Extends <see cref="Mock{IUrlHelper}"/> with functionality to help setup and verify invocations, except in terms of the extension
    /// methods of <see cref="IUrlHelper"/> that are provided by <see cref="UrlHelperExtensions"/>.
    /// </summary>
    public static class MockUrlHelperExtensions
    {
        /// <summary>
        /// Adds an invocation setup to <paramref name="mockUrlHelper"/> to match an invocation that would be generated by <see cref="UrlHelperExtensions.RouteUrl(IUrlHelper, string)"/>.
        /// </summary>
        /// <param name="mockUrlHelper">The mock to add the invocation setup to.</param>
        /// <param name="routeName">The name of the route that invocations must match.</param>
        /// <returns>Returns an <see cref="ISetup{IUrlHelper, string}"/> so that additional configuration calls can be chained.</returns>
        public static ISetup<IUrlHelper, string> SetupRouteUrl(this Mock<IUrlHelper> mockUrlHelper, string routeName)
        {
            var urlRouteContextPredicateExpression = UrlRouteContextPredicateLambdaExpressionFactory.Create(routeName);

            return mockUrlHelper.SetupRouteUrl(urlRouteContextPredicateExpression);
        }

        /// <summary>
        /// Adds an invocation setup to <paramref name="mockUrlHelper"/> to match an invocation that would be generated by <see cref="UrlHelperExtensions.RouteUrl(IUrlHelper, string, object)"/>.
        /// </summary>
        /// <typeparam name="TRouteValues">The type of route values being provided as the comparand for invocation setup.</typeparam>
        /// <param name="mockUrlHelper">The mock to add the invocation setup to.</param>
        /// <param name="routeName">The name of the route that invocations must match.</param>
        /// <param name="routeValues">The route values that invocations must match.</param>
        /// <returns>Returns an <see cref="ISetup{IUrlHelper, string}"/> so that additional configuration calls can be chained.</returns>
        public static ISetup<IUrlHelper, string> SetupRouteUrl<TRouteValues>(this Mock<IUrlHelper> mockUrlHelper, string routeName, TRouteValues routeValues)
        {
            var urlRouteContextPredicateExpression = UrlRouteContextPredicateLambdaExpressionFactory.Create(routeName, routeValues);

            return mockUrlHelper.SetupRouteUrl(urlRouteContextPredicateExpression);
        }

        /// <summary>
        /// Performs a verification on the <paramref name="mockUrlHelper"/> that it has recieved an invocation matching that which would be generated by <see cref="UrlHelperExtensions.RouteUrl(IUrlHelper, string)"/>.
        /// </summary>
        /// <param name="mockUrlHelper">The mock to add the invocation setup to.</param>
        /// <param name="routeName">The name of the route that invocations must match.</param>
        /// <param name="times">The number of times the invocation must match in order for the verification to pass.</param>
        public static void VerifyRouteUrl(this Mock<IUrlHelper> mockUrlHelper, string routeName, Times times)
        {
            var urlRouteContextPredicateExpression = UrlRouteContextPredicateLambdaExpressionFactory.Create(routeName);

            mockUrlHelper.VerifyRouteUrl(urlRouteContextPredicateExpression, times);
        }

        /// <summary>
        /// Performs a verification on the <paramref name="mockUrlHelper"/> that it has recieved an invocation matching that which would be generated by <see cref="UrlHelperExtensions.RouteUrl(IUrlHelper, string, object)"/>.
        /// </summary>
        /// <typeparam name="TRouteValues">The type of route values being provided as the comparand for invocation verification request.</typeparam>
        /// <param name="mockUrlHelper">The mock to add the invocation setup to.</param>>
        /// <param name="routeName">The name of the route that invocations must match.</param>
        /// <param name="routeValues">The route values that invocations must match.</param>
        /// <param name="times">The number of times the invocation must match in order for the verification to pass.</param>
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