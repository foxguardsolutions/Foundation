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

using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    [Unit]
    [TestFixture]
    public class ExceptionFilterWrapperTests
    {
        [Test]
        public void RequiresFilterMetadata()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new ExceptionFilterWrapper(null));
            Assert.That(exception.ParamName, Is.EqualTo("filterMetadata"));
        }

        [Test]
        public async Task WrapperResolvesExceptionFilterFromDependencyScope()
        {
            var builder = new ContainerBuilder();
            builder.Register<ILogger>(c => new Logger()).InstancePerDependency();
            var activationCount = 0;
            builder.Register(c => new TestExceptionFilter(c.Resolve<ILogger>()))
                .AsWebApiExceptionFilterWhen((hcd, had) => typeof(TestController).IsAssignableFrom(had.ControllerDescriptor.ControllerType) && had.ActionName == nameof(TestController.Get), FilterScope.Action)
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
            var actionExecutedContext = new HttpActionExecutedContext(actionContext, null);
            var metadata = new CustomWebApiFilterMetadata()
            {
                Predicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType),
                FilterScope = FilterScope.Action
            };
            var wrapper = new ExceptionFilterWrapper(metadata);

            await wrapper.OnExceptionAsync(actionExecutedContext, CancellationToken.None);
            Assert.That(activationCount, Is.EqualTo(1));
        }
    }
}
