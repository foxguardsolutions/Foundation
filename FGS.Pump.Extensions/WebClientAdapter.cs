using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FGS.Pump.Extensions
{
    public class WebClientAdapter : IWebClient
    {
        private readonly WebClient _innerClient;

        public WebClientAdapter(WebClient client)
        {
            _innerClient = client;
        }

        public Task<Stream> OpenReadTaskAsync(Uri address)
        {
            return _innerClient.OpenReadTaskAsync(address);
        }

        public byte[] UploadValues(string address, string method, NameValueCollection data)
        {
            return _innerClient.UploadValues(address, method, data);
        }
    }
}
