using System.IO;

namespace FGS.Pump.Extensions
{
    public static class StreamExtensions
    {
        public static MemoryStream CopyToMemory(this Stream self)
        {
            var result = new MemoryStream();
            self.CopyTo(result);
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }
    }
}
