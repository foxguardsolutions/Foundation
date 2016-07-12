using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/TestTypes.cs </remarks>
    public class TestModelBinder : IModelBinder
    {
        public Dependency Dependency { get; private set; }

        public TestModelBinder(Dependency dependency)
        {
            Dependency = dependency;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            return true;
        }
    }
}