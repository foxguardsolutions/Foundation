using System;

using RestSharp;
using RestSharp.Deserializers;

namespace FGS.Pump.Extensions
{
    public class NullDeserializer : IDeserializer
    {
        public T Deserialize<T>(IRestResponse response)
        {
            throw new InvalidOperationException();
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
    }
}
