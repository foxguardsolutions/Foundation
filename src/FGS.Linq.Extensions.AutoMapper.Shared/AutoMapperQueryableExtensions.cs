using System.Linq;
using AutoMapper;

namespace FGS.Linq.Extensions.AutoMapper
{
    /// <summary>
    /// Extends <see cref="IQueryable{T}"/> with the ability to transform it into a query of a different type, using AutoMapper.
    /// </summary>
    public static class AutoMapperQueryableExtensions
    {
        /// <summary>
        /// Projects a query into a new output type, using <paramref name="mapper"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of items in the source query.</typeparam>
        /// <typeparam name="TDestination">The type of items in the result query.</typeparam>
        /// <param name="source">The original query that is being transformed.</param>
        /// <param name="mapper">The mapper that converts the query.</param>
        /// <returns>A query of items of type <typeparamref name="TDestination"/>.</returns>
        public static IQueryable<TDestination> Map<TSource, TDestination>(this IQueryable<TSource> source, IMapper mapper)
        {
#if PROJECTTO_IS_EXTENSION
            return global::AutoMapper.QueryableExtensions.Extensions.ProjectTo<TDestination>(source, mapper.ConfigurationProvider);
#elif PROJECTTO_IS_MEMBER
            return mapper.ProjectTo<TDestination>(source);
#else
            throw new NotImplementedException();
#endif
        }
    }
}
