using System;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Hosting;

using Autofac;
using Autofac.Integration.WebApi;

using FGS.Pump.Tests.Support.TestCategories;

using Moq;

using NUnit.Framework;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/AuthenticationFilterWrapperFixture.cs </remarks>
    [Unit]
    [TestFixture]
    public class AuthenticationFilterWrapperTests
    {
        [Test]
        public void RequiresFilterMetadata()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new AuthenticationFilterWrapper(null));
            Assert.That(exception.ParamName, Is.EqualTo("filterMetadata"));
        }

        [Test]
        public async Task WrapperResolvesAuthenticationFilterFromDependencyScope()
        {
            var builder = new ContainerBuilder();
            builder.Register<ILogger>(c => new Logger()).InstancePerDependency();
            var activationCount = 0;
            builder.Register(c => new TestAuthenticationFilter(c.Resolve<ILogger>()))
                .AsWebApiAuthenticationFilterWhen((hcd, had) => typeof(TestController).IsAssignableFrom(had.ControllerDescriptor.ControllerType) && had.ActionName == nameof(TestController.Get), FilterScope.Action)
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
            var authenticationContext = new HttpAuthenticationContext(actionContext, default(IPrincipal));
            var authenticationChallengeContext = new HttpAuthenticationChallengeContext(actionContext, new Mock<IHttpActionResult>().Object);
            var metadata = new CustomWebApiFilterMetadata()
            {
                Predicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType),
                FilterScope = FilterScope.Action
            };
            var wrapper = new AuthenticationFilterWrapper(metadata);

            await wrapper.AuthenticateAsync(authenticationContext, CancellationToken.None);

            Assert.That(activationCount, Is.EqualTo(1));

            await wrapper.ChallengeAsync(authenticationChallengeContext, CancellationToken.None);

            Assert.That(activationCount, Is.EqualTo(1));
        }
    }
}
