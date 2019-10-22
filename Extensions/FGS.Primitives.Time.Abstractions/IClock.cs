using System;

namespace FGS.Primitives.Time.Abstractions
{
    public interface IClock
    {
        DateTimeOffset Now { get; }

        DateTimeOffset UtcNow { get; }
    }
}
