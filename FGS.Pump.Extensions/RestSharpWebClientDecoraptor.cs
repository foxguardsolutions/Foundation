using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using RestSharp;

namespace FGS.Pump.Extensions
{
    public class RestSharpWebClientDecoraptor : IWebClient
    {
        private readonly IRestClientFactory _clientFactory;
        private readonly Func<Uri, Method, IRestRequest> _restRequestFactory;
        private readonly Func<Stream, Stream> _streamCopyFactory;

        public X509Certificate2 ClientCertificate { get; set; }

        public RestSharpWebClientDecoraptor(
            IRestClientFactory clientFactory,
            Func<Uri, Method, IRestRequest> restRequestFactory,
            Func<Stream, Stream> streamCopyFactory)
        {
            _clientFactory = clientFactory;
            _restRequestFactory = restRequestFactory;
            _streamCopyFactory = streamCopyFactory;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<Stream> OpenReadTaskAsync(Uri address)
        {
            var client = _clientFactory.Create(address, ClientCertificate);
            var request = _restRequestFactory(address, Method.GET);
            var result = default(Stream);
            request.ResponseWriter = (stream) =>
            {
                result = _streamCopyFactory(stream);
            };
            await client.ExecuteTaskAsync(request);
            return result;
        }

        public byte[] UploadValues(string address, string method, NameValueCollection data)
        {
            var location = new Uri(address);
            var client = _clientFactory.Create(location, ClientCertificate);

            var methodValue = GetRestMethodFromString(method);
            var request = _restRequestFactory(location, methodValue);
            request.AddJsonBody(data);

            var response = client.Execute(request);

            return response.RawBytes;
        }

        public async Task<string> DownloadStringTaskAsync(Uri address)
        {
            var client = _clientFactory.Create(address, ClientCertificate);
            var request = _restRequestFactory(address, Method.GET);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        private static Method GetRestMethodFromString(string method)
        {
            Method methodValue;
            if (!Enum.TryParse(method, true, out methodValue))
                throw new ArgumentOutOfRangeException(nameof(method));
            return methodValue;
        }
    }
}
