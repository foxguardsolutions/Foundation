using System;

namespace FGS.Pump.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static DateTimeOffset FloorToDay(this DateTimeOffset dateTimeOffset)
        {
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, 0, 0, 0, dateTimeOffset.Offset);
        }

        public static DateTimeOffset RoundToSecond(this DateTimeOffset dateTimeOffset)
        {
            var timeOfDay = TimeSpan.FromSeconds(Math.Round(dateTimeOffset.TimeOfDay.TotalSeconds));
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, timeOfDay.Hours, timeOfDay.Minutes, timeOfDay.Seconds, dateTimeOffset.Offset);
        }

        public static DateTimeOffset FloorToSecond(this DateTimeOffset dateTimeOffset)
        {
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Offset);
        }

        public static double ToUnixTimestampSeconds(this DateTimeOffset value)
        {
            return TimeSpan.FromTicks(value.ToUniversalTime().Ticks - UnixEpoch.ToUniversalTime().Ticks).TotalSeconds;
        }
    }
}
