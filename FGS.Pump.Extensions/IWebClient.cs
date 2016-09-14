using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FGS.Pump.Extensions
{
    public interface IWebClient : IDisposable
    {
        X509Certificate2 ClientCertificate { get; set; }

        Task<Stream> OpenReadTaskAsync(Uri address);
        byte[] UploadValues(string address, string method, NameValueCollection data);

        Task<string> DownloadStringTaskAsync(Uri address);
    }
}
