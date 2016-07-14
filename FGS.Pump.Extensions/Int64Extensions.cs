using System;

namespace FGS.Pump.Extensions
{
    public static class Int64Extensions
    {
        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <remarks>
        /// Borrowed from http://stackoverflow.com/questions/14488796/does-net-provide-an-easy-way-convert-bytes-to-kb-mb-gb-etc
        /// </remarks>>
        public static string SizeSuffix(this long value)
        {
            if (value < 0)
            {
                return "-" + SizeSuffix(-value);
            }

            if (value == 0)
            {
                return "0.0 bytes";
            }

            var mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            if (mag == 0)
            {
                return $"{adjustedSize:n0} {SizeSuffixes[mag]}";
            }

            return $"{adjustedSize:n1} {SizeSuffixes[mag]}";
        }
    }
}