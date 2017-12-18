using System;
using System.Security.Cryptography.X509Certificates;

using RestSharp;

namespace FGS.Pump.Extensions
{
    public class MetaRestClientFactoryDecorator : IRestClientFactory
    {
        private readonly IRestClientFactory _decorated;
        public MetaRestClientFactoryDecorator(IRestClientFactory decorated)
        {
            _decorated = decorated;
        }

        public IRestClient Create(Uri address)
        {
            var client = _decorated.Create(address);

            AddHandler(client);

            return client;
        }

        public IRestClient Create(Uri address, X509Certificate2 clientCertificate)
        {
            var client = _decorated.Create(address, clientCertificate);

            AddHandler(client);

            return client;
        }

        private void AddHandler(IRestClient client)
        {
            client.AddHandler("text/plain", new NullDeserializer());
        }
    }
}
