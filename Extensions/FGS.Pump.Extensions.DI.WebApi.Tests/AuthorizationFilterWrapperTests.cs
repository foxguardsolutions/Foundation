using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Hosting;

using Autofac;
using Autofac.Integration.WebApi;

using FGS.Pump.Extensions.DI.WebApi.Tests.TestTypes;
using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/AuthorizationFilterWrapperFixture.cs </remarks>
    [Unit]
    [TestFixture]
    public class AuthorizationFilterWrapperTests
    {
        [Test]
        public void RequiresFilterMetadata()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new AuthorizationFilterWrapper(null));
            Assert.That(exception.ParamName, Is.EqualTo("filterMetadata"));
        }

        [Test]
        public async Task WrapperResolvesAuthorizationFilterFromDependencyScope()
        {
            var builder = new ContainerBuilder();
            builder.Register<ILogger>(c => new Logger()).InstancePerDependency();
            var activationCount = 0;
            builder.Register(c => new TestAuthorizationFilter(c.Resolve<ILogger>()))
                .AsWebApiAuthorizationFilterWhen((hcd, had) => typeof(TestController).IsAssignableFrom(had.ControllerDescriptor.ControllerType) && had.ActionName == nameof(TestController.Get), FilterScope.Action, order: 0)
                .InstancePerRequest()
                .OnActivated(e => activationCount++);
            var container = builder.Build();

            var resolver = new AutofacWebApiDependencyResolver(container);
            var configuration = new HttpConfiguration { DependencyResolver = resolver };
            var requestMessage = new HttpRequestMessage();
            requestMessage.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, configuration);
            var contollerContext = new HttpControllerContext { Request = requestMessage };
            var controllerDescriptor = new HttpControllerDescriptor { ControllerType = typeof(TestController) };
            var methodInfo = typeof(TestController).GetMethod("Get");
            var actionDescriptor = new ReflectedHttpActionDescriptor(controllerDescriptor, methodInfo);
            var actionContext = new HttpActionContext(contollerContext, actionDescriptor);
            var metadata = new CustomWebApiFilterMetadata()
            {
                Predicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType),
                FilterScope = FilterScope.Action
            };
            var wrapper = new AuthorizationFilterWrapper(metadata);

            await wrapper.OnAuthorizationAsync(actionContext, CancellationToken.None);

            Assert.That(activationCount, Is.EqualTo(1));
        }
    }
}
