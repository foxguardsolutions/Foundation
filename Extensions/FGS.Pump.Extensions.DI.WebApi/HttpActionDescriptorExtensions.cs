using System.Web.Http.Controllers;

using Autofac;
using Autofac.Core;

namespace FGS.Pump.Extensions.DI.WebApi
{
    internal static class HttpActionDescriptorExtensions
    {
        private const string ParameterNamePrefix = "CustomAutofacWebApi";
        internal const string HttpControllerDescriptorParameterName = ParameterNamePrefix + "HttpControllerDescriptor";
        internal const string HttpActionDescriptorParameterName = ParameterNamePrefix + "HttpActionDescriptor";

        internal static Parameter[] CreateFilterResolutionParameters(this HttpActionDescriptor actionDescriptor)
        {
            var resolveParameters = new Parameter[]
                                        {
                                            new NamedParameter(HttpActionDescriptorParameterName, actionDescriptor),
                                            new NamedParameter(HttpControllerDescriptorParameterName, actionDescriptor.ControllerDescriptor)
                                        };
            return resolveParameters;
        }
    }
}
