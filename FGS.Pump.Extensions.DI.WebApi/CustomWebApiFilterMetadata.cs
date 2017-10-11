using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FGS.Pump.Extensions.DI.WebApi
{
    internal class CustomWebApiFilterMetadata
    {
        public Func<HttpControllerDescriptor, HttpActionDescriptor, bool> Predicate { get; set; }

        public FilterScope FilterScope { get; set; }

        public int Order { get; set; }
    }
}
