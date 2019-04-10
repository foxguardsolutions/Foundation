using System;

namespace FGS.Pump.Extensions
{
    public class SystemClock : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.Now;

        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}