using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Hosting;

using Autofac;

using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    [Unit]
    [TestFixture]
    public class ActionFilterWrapperTests
    {
        [Test]
        public void RequiresFilterMetadata()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new ActionFilterWrapper(null));
            Assert.That(exception.ParamName, Is.EqualTo("filterMetadata"));
        }

        [Test]
        public async Task WrapperResolvesActionFilterFromDependencyScope()
        {
            var builder = new ContainerBuilder();
            builder.Register<ILogger>(c => new Logger()).InstancePerDependency();
            var activationCount = 0;
            Func<HttpControllerDescriptor, HttpActionDescriptor, bool> predicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType) && had.ActionName == nameof(TestController.Get);
            builder.Register<ICustomAutofacActionFilter>(c => new TestActionFilter(c.Resolve<ILogger>()))
                .AsWebApiActionFilterWhen(predicate, FilterScope.Action, order: 0)
                .InstancePerRequest()
                .OnActivated(e => activationCount++);
            var container = builder.Build();

            var resolver = new Autofac.Integration.WebApi.AutofacWebApiDependencyResolver(container);
            var configuration = new HttpConfiguration { DependencyResolver = resolver };
            var requestMessage = new HttpRequestMessage();
            requestMessage.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, configuration);
            var contollerContext = new HttpControllerContext { Request = requestMessage };
            var controllerDescriptor = new HttpControllerDescriptor { ControllerType = typeof(TestController) };
            var methodInfo = typeof(TestController).GetMethod("Get");
            var actionDescriptor = new ReflectedHttpActionDescriptor(controllerDescriptor, methodInfo);
            var httpActionContext = new HttpActionContext(contollerContext, actionDescriptor);
            var actionContext = new HttpActionContext(contollerContext, actionDescriptor);
            var httpActionExecutedContext = new HttpActionExecutedContext(actionContext, null);
            var metadata = new CustomWebApiFilterMetadata()
            {
                FilterScope = FilterScope.Action,
                Predicate = predicate
            };
            var wrapper = new ActionFilterWrapper(metadata);

            await wrapper.OnActionExecutingAsync(httpActionContext, CancellationToken.None);
            Assert.That(activationCount, Is.EqualTo(1));

            await wrapper.OnActionExecutedAsync(httpActionExecutedContext, CancellationToken.None);
            Assert.That(activationCount, Is.EqualTo(1));
        }

        [Test]
        public void ReturnsCorrectMetadataKey()
        {
            var wrapper = new ActionFilterWrapper(new CustomWebApiFilterMetadata());
            Assert.That(wrapper.MetadataKey, Is.EqualTo(CustomAutofacWebApiFilterProvider.ActionFilterMetadataKey));
        }
    }
}
