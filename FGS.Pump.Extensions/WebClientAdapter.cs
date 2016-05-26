using System;
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
    }
}
