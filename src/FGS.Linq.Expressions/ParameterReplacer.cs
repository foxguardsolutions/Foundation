using System.Linq;
using System.Linq.Expressions;

namespace FGS.Linq.Expressions
{
    /// <summary>Provides the ability to replace expression tree parameters with other expressions.</summary>
    /// <remarks>Taken and modified from: https://stackoverflow.com/questions/11159697/replace-parameter-in-lambda-expression. </remarks>
    public static class ParameterReplacer
    {
        /// <summary>
        /// Produces an expression identical to <paramref name="expression"/>, except with all occurrences of the <paramref name="source"/>
        /// parameter replaced with the <paramref name="target"/> expression.
        /// </summary>
        /// <typeparam name="TInput">The type of delegate that <paramref name="expression"/> is an expression of.</typeparam>
        /// <typeparam name="TOutput">The type of delegate that the result will be an expression of.</typeparam>
        /// <param name="expression">The expression whose contents will be partially replaced.</param>
        /// <param name="source">The expression which will be sought within <paramref name="expression"/> and replaced with <paramref name="target"/>.</param>
        /// <param name="target">The expression which will be substituted for <paramref name="target"/> when it is found within <paramref name="expression"/>.</param>
        /// <returns>An expression that has been transformed from <paramref name="expression"/> in order to have all occurrences of <paramref name="source"/> replaced with <paramref name="target"/>.</returns>
        public static Expression<TOutput> Replace<TInput, TOutput>(
                        Expression<TInput> expression,
                        ParameterExpression source,
                        Expression target)
        {
            return new ParameterReplacerVisitor<TOutput>(source, target)
                        .VisitAndConvert(expression);
        }

        private class ParameterReplacerVisitor<TOutput> : ExpressionVisitor
        {
            private ParameterExpression _source;
            private Expression _target;

            public ParameterReplacerVisitor(ParameterExpression source, Expression target)
            {
                _source = source;
                _target = target;
            }

            internal Expression<TOutput> VisitAndConvert<T>(Expression<T> root)
            {
                return (Expression<TOutput>)VisitLambda(root);
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                // Leave all parameters alone except the one we want to replace.
                var parameters = node.Parameters.Where(p => p != _source);

                return Expression.Lambda<TOutput>(Visit(node.Body), parameters);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                // Replace the source with the target, visit other params as usual.
                return node == _source ? _target : base.VisitParameter(node);
            }
        }
    }
}
