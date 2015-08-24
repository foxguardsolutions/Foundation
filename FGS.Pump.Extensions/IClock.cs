using System;

namespace FGS.Pump.Extensions
{
    public interface IClock
    {
        DateTimeOffset Now { get; }

        DateTimeOffset UtcNow { get; }
    }
}