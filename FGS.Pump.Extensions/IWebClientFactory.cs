using System.Security.Cryptography.X509Certificates;

namespace FGS.Pump.Extensions
{
    public interface IWebClientFactory
    {
        IWebClient Create();
        IWebClient Create(X509Certificate2 clientCertificate);
    }
}