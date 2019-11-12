using System;

namespace FGS.Collections.Extensions.Pagination.Abstractions
{
    /// <summary>
    /// Specifies how a set of items can be subdivided into sequential pages, and then a specific page of them individually selected.
    /// </summary>
    public struct PaginationSpecification : IEquatable<PaginationSpecification>
    {
        /// <summary>
        /// Gets the 0-based ordinal of the page to be selected.
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// Gets the size of pages to subdivide the source into.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationSpecification"/> struct.
        /// </summary>
        /// <param name="pageNumber">The 0-based ordinal of the page to be selected.</param>
        /// <param name="pageSize">The size of pages to subdivide the source into.</param>
        public PaginationSpecification(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Initializes and returns a new instance of the  <see cref="PaginationSpecification"/> struct, pointing
        /// to the first page and where pages are of size <paramref name="pageSize"/>.
        /// </summary>
        /// <param name="pageSize">The size of pages to subdivide the source into.</param>
        /// <returns>A newly initialized instance of the <see cref="PaginationSpecification"/> struct.</returns>
        public static PaginationSpecification FirstPageOfSize(int pageSize) => new PaginationSpecification(0, pageSize);

        /// <summary>
        /// Generates the <see cref="PaginationSpecification"/> that points to the next page of the same size as the current one.
        /// </summary>
        /// <returns>A newly initialized instance of the <see cref="PaginationSpecification"/> struct, which points at the next page.</returns>
        public PaginationSpecification Next() => new PaginationSpecification(PageNumber + 1, PageSize);

        /// <summary>
        /// Generates the <see cref="PaginationSpecification"/> that points to the previous page of the same size as the current one,
        /// or <value>null</value> if this is the first page.
        /// </summary>
        /// <returns>A newly initialized instance of the <see cref="PaginationSpecification"/> struct, which points at the previous page if one exists, or <value>null</value> if it does not.</returns>
        public PaginationSpecification? Previous()
        {
            return PageNumber <= 0
                ? (PaginationSpecification?)null
                : new PaginationSpecification(PageNumber - 1, PageSize);
        }

        /// <inheritdoc cref="IEquatable{PaginationSpecification}.Equals(PaginationSpecification)"/>
        public bool Equals(PaginationSpecification other) => PageNumber == other.PageNumber && PageSize == other.PageSize;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is null
                ? false
                : obj is PaginationSpecification specification && Equals(specification);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (PageNumber * 397) ^ PageSize;
            }
        }

        /// <summary>
        /// Checks two instances of <see cref="PaginationSpecification"/> for equality.
        /// </summary>
        /// <returns>A value indicating whether or not the two instances of <see cref="PaginationSpecification"/> are equal to each other.</returns>
        public static bool operator ==(PaginationSpecification left, PaginationSpecification right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks two instances of <see cref="PaginationSpecification"/> for equality, and returns the opposite of such.
        /// </summary>
        /// <returns>A value indicating the opposite of whether or not the two instances of <see cref="PaginationSpecification"/> are equal to each other.</returns>
        public static bool operator !=(PaginationSpecification left, PaginationSpecification right)
        {
            return !(left == right);
        }
    }
}
