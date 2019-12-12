using OneOf;
using OneOf.Types;
using System;

namespace FGS.OneOf.Extensions
{
    public static class OneOfExtensions
    {
        public static OneOf<TSuccessNew, TError> ContinueOnSuccessWith<TSuccessOriginal, TError, TSuccessNew>(
          this OneOf<TSuccessOriginal, TError> original,
          Func<TSuccessOriginal, OneOf<TSuccessNew, TError>> continuation)
        {
            return original.Match(
                originalSuccess => continuation(originalSuccess).Match<OneOf<TSuccessNew, TError>>(newSuccess => newSuccess, newError => newError),
                originalError => originalError);
        }

        public static OneOf<TSuccess, Error<string>> SelectErrorString<TSuccess, TErrorOriginal>(
            this OneOf<TSuccess, TErrorOriginal> source,
            Func<TErrorOriginal, string> projection)
        {
            return source.SelectError(e => new Error<string>(projection(e)));
        }
    }
}
