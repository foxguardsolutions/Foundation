using System;
using System.Security.Cryptography.X509Certificates;

using RestSharp;

namespace FGS.Pump.Extensions
{
    public class RestClientFactory : IRestClientFactory
    {
        private readonly Func<IRestClient> _innerFactory;
        private readonly Func<X509Certificate2, X509Certificate2Collection> _certificateCollectionFactory;

        public RestClientFactory(Func<IRestClient> innerFactory, Func<X509Certificate2, X509Certificate2Collection> certificateCollectionFactory)
        {
            _innerFactory = innerFactory;
            _certificateCollectionFactory = certificateCollectionFactory;
        }

        public IRestClient Create(Uri address)
        {
            var client = _innerFactory();
            client.BaseUrl = new Uri($"{address.Scheme}://{address.Authority}");
            return client;
        }

        public IRestClient Create(Uri address, X509Certificate2 clientCertificate)
        {
            var client = Create(address);
            if (clientCertificate != default(X509Certificate2))
                client.ClientCertificates = _certificateCollectionFactory(clientCertificate);
            return client;
        }
    }
}
