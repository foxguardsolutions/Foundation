using System;

using FGS.Primitives.Time.Abstractions;

namespace FGS.Primitives.Time
{
    public class SystemClock : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.Now;

        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
