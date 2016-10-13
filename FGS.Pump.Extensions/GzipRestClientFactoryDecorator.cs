using System;
using System.Security.Cryptography.X509Certificates;

using RestSharp;
using RestSharp.Deserializers;

namespace FGS.Pump.Extensions
{
    public class GzipRestClientFactoryDecorator : IRestClientFactory
    {
        private readonly IRestClientFactory _decorated;
        public GzipRestClientFactoryDecorator(IRestClientFactory decorated)
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
            client.AddHandler("application/x-gzip", new NullDeserializer());
        }
    }
}
