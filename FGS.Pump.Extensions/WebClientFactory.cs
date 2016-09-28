using System;
using System.Security.Cryptography.X509Certificates;

namespace FGS.Pump.Extensions
{
    public class WebClientFactory : IWebClientFactory
    {
        private readonly Func<IWebClient> _webClientFactory;

        public WebClientFactory(Func<IWebClient> webClientFactory)
        {
            _webClientFactory = webClientFactory;
        }

        public IWebClient Create()
        {
            return Create(null);
        }

        public IWebClient Create(X509Certificate2 clientCertificate)
        {
            var client = _webClientFactory();
            client.ClientCertificate = clientCertificate;
            return client;
        }
    }
}
