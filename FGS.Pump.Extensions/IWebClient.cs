using System;
using System.IO;
using System.Threading.Tasks;

namespace FGS.Pump.Extensions
{
    public interface IWebClient
    {
        Task<Stream> OpenReadTaskAsync(Uri address);
    }
}
