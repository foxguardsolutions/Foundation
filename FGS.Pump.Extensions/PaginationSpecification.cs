using System;

namespace FGS.Pump.Extensions
{
    public struct PaginationSpecification : IEquatable<PaginationSpecification>
    {
        public int PageNumber { get; }

        public int PageSize { get; }

        public PaginationSpecification(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public static PaginationSpecification FirstPageOfSize(int pageSize) => new PaginationSpecification(0, pageSize);

        public PaginationSpecification Next() => new PaginationSpecification(PageNumber + 1, PageSize);

        public PaginationSpecification? Previous()
        {
            return PageNumber <= 0
                ? (PaginationSpecification?)null
                : new PaginationSpecification(PageNumber - 1, PageSize);
        }

        public bool Equals(PaginationSpecification other) => PageNumber == other.PageNumber && PageSize == other.PageSize;

        public override bool Equals(object obj)
        {
            return obj is null
                ? false
                : obj is PaginationSpecification specification && Equals(specification);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PageNumber * 397) ^ PageSize;
            }
        }
    }
}