using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FGS.Linq.Expressions
{
    /// <summary>
    /// Provides extension methods for <see cref="IQueryProvider"/>.
    /// </summary>
    public static class QueryProviderExtensions
    {
        /// <summary>
        /// Wraps a non-<see cref="IQueryable{T}"/> query as a subselect and returns it as an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the single row that will be in the result of the query.</typeparam>
        /// <param name="queryProvider">The <see cref="IQueryProvider"/> that can translate expression trees into queries against an underlying storage mechanism.</param>
        /// <param name="selectExpression">An expression that selects a single value or anonymous object that is projected via expressions that can be converted into one or more subqueries.</param>
        /// <returns>An <see cref="IQueryable{T}"/> that selects a single row of one or more values that are projected by subqueries.</returns>
        /// <remarks>
        /// <para>
        /// Taken and modified from: https://stackoverflow.com/a/8996021.
        /// </para>
        /// <para>
        /// Callers are responsible for authoring subqueries in such a way that the given <paramref name="queryProvider"/> can interpret it.
        /// </para>
        /// </remarks>
        public static IQueryable<TResult> CreateScalarQuery<TResult>(this IQueryProvider queryProvider, Expression<Func<TResult>> selectExpression)
        {
            MethodInfo GetMethodInfo(Expression<Action> lambdaOfMethodCallExpression)
            {
                return ((MethodCallExpression)lambdaOfMethodCallExpression.Body).Method;
            }

            return queryProvider.CreateQuery<TResult>(
                Expression.Call(
                    method: GetMethodInfo(() => Queryable.Select(null, (Expression<Func<int, TResult>>)null)),
                    arg0: Expression.Call(
                        method: GetMethodInfo(() => Queryable.AsQueryable<int>(null)),
                        arg0: Expression.NewArrayInit(typeof(int), Expression.Constant(1))),
                    arg1: Expression.Lambda(body: selectExpression.Body, parameters: new[] { Expression.Parameter(typeof(int)) })));
        }
    }
}
