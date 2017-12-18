using System;
using System.Security.Cryptography.X509Certificates;

using RestSharp;

namespace FGS.Pump.Extensions
{
    public interface IRestClientFactory
    {
        IRestClient Create(Uri address);
        IRestClient Create(Uri address, X509Certificate2 clientCertificate);
    }
}