using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;

namespace FGS.Pump.Extensions
{
    public interface IWebClient
    {
        Task<Stream> OpenReadTaskAsync(Uri address);
        byte[] UploadValues(string address, string method, NameValueCollection data);
    }
}
