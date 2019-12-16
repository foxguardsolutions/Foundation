using OneOf;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FGS.OneOf.Extensions
{
    public static class OneOfQueryExpressionExtensions
    {
        public static OneOf<TSuccess, TErrorNew> SelectError<TSuccess, TErrorOriginal, TErrorNew>(
            this OneOf<TSuccess, TErrorOriginal> source,
            Func<TErrorOriginal, TErrorNew> projection)
        {
            return source.Match<OneOf<TSuccess, TErrorNew>>(success => success, error => projection(error));
        }

        public static OneOf<TSuccessNew, TError> Select<TSuccessOriginal, TSuccessNew, TError>(
            this OneOf<TSuccessOriginal, TError> source,
            Func<TSuccessOriginal, TSuccessNew> projection)
        {
            return source.Match<OneOf<TSuccessNew, TError>>(success => projection(success), error => error);
        }

        public static OneOf<TResult, TError> SelectMany<TSuccess1, TSuccess2, TError, TResult>(
            this OneOf<TSuccess1, TError> source,
            Func<TSuccess1, OneOf<TSuccess2, TError>> selector,
            Func<TSuccess1, TSuccess2, TResult> resultSelector)
        {
            return source.Match(
                sourceSuccess =>
                    selector(sourceSuccess).Match<OneOf<TResult, TError>>(
                        secondSuccess => resultSelector(sourceSuccess, secondSuccess),
                        error => error),
                error => error);
        }

        public static OneOf<IEnumerable<TResult>, TError> SelectMany<TSource, TSuccess, TError, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, OneOf<TSuccess, TError>> selector,
            Func<TSource, TSuccess, TResult> resultSelector)
        {
            return source.Aggregate(
                OneOf<ImmutableArray<TResult>, TError>.FromT0(ImmutableArray<TResult>.Empty),
                (previousAggregate, currentItem) =>
                    previousAggregate.Match(
                        previousSuccesses => selector(currentItem).Match<OneOf<ImmutableArray<TResult>, TError>>(
                            itemSuccess => previousSuccesses.Add(resultSelector(currentItem, itemSuccess)),
                            itemError => itemError),
                        previousError => previousError),
                aggregate => aggregate.Match<OneOf<IEnumerable<TResult>, TError>>(allSuccesses => allSuccesses, error => error));
        }

        public static OneOf<TSuccess, TError> Flatten<TSuccess, TError>(this OneOf<OneOf<TSuccess, TError>, TError> source)
        {
            return source.Match(inner => inner.Match<OneOf<TSuccess, TError>>(success => success, error => error), error => error);
        }

        public static T Bifold<T>(this OneOf<T, T> source)
        {
            return source.Match(success => success, error => error);
        }
    }
}
