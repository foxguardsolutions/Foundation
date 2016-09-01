using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Autofac;

using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/AutofacWebApiFilterProviderFixture.cs </remarks>
    [Unit]
    [TestFixture]
    public class CustomAutofacWebApiFilterProviderFixture
    {
        private readonly Func<HttpControllerDescriptor, HttpActionDescriptor, bool> _controllerPredicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType);
        private readonly Func<HttpControllerDescriptor, HttpActionDescriptor, bool> _actionPredicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType) && had.ActionName == nameof(TestController.Get);

        [Test]
        public void FilterRegistrationsWithoutMetadataIgnored()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AuthorizeAttribute>().AsImplementedInterfaces();
            var container = builder.Build();
            var provider = new CustomAutofacWebApiFilterProvider(container);
            var configuration = new HttpConfiguration { DependencyResolver = new Autofac.Integration.WebApi.AutofacWebApiDependencyResolver(container) };
            var actionDescriptor = BuildActionDescriptorForGetMethod();

            var filterInfos = provider.GetFilters(configuration, actionDescriptor);

            Assert.False(filterInfos.Select(f => f.Instance).OfType<AuthorizeAttribute>().Any());
        }

        [Test]
        public void InjectsFilterPropertiesForRegisteredDependencies()
        {
            var builder = new ContainerBuilder();
            builder.Register<ILogger>(c => new Logger()).InstancePerDependency();
            var container = builder.Build();
            var provider = new CustomAutofacWebApiFilterProvider(container);
            var configuration = new HttpConfiguration { DependencyResolver = new Autofac.Integration.WebApi.AutofacWebApiDependencyResolver(container) };
            var actionDescriptor = BuildActionDescriptorForGetMethod();

            var filterInfos = provider.GetFilters(configuration, actionDescriptor).ToArray();

            var filter = filterInfos.Select(info => info.Instance).OfType<CustomActionFilter>().Single();
            Assert.That(filter.Logger, Is.AssignableTo<ILogger>());
        }

        [Test]
        public void ReturnsFiltersWithoutPropertyInjectionForUnregisteredDependencies()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();
            var provider = new CustomAutofacWebApiFilterProvider(container);
            var configuration = new HttpConfiguration { DependencyResolver = new Autofac.Integration.WebApi.AutofacWebApiDependencyResolver(container) };
            var actionDescriptor = BuildActionDescriptorForGetMethod();

            var filterInfos = provider.GetFilters(configuration, actionDescriptor).ToArray();

            var filter = filterInfos.Select(info => info.Instance).OfType<CustomActionFilter>().Single();
            Assert.Null(filter.Logger);
        }

        [Test]
        public void CanRegisterMultipleFilterTypesAgainstSingleService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new TestCombinationFilter())
                .AsWebApiActionFilterWhen(_controllerPredicate, FilterScope.Controller)
                .AsWebApiAuthenticationFilterWhen(_controllerPredicate, FilterScope.Controller)
                .AsWebApiAuthorizationFilterWhen(_controllerPredicate, FilterScope.Controller)
                .AsWebApiExceptionFilterWhen(_controllerPredicate, FilterScope.Controller);
            var container = builder.Build();

            Assert.NotNull(container.Resolve<ICustomAutofacActionFilter>());
            Assert.NotNull(container.Resolve<ICustomAutofacAuthenticationFilter>());
            Assert.NotNull(container.Resolve<ICustomAutofacAuthorizationFilter>());
            Assert.NotNull(container.Resolve<ICustomAutofacExceptionFilter>());
        }

        [Test]
        public void ResolvesMultipleFiltersOfDifferentTypes()
        {
            var builder = new ContainerBuilder();
            builder.Register<ILogger>(c => new Logger()).InstancePerDependency();

            builder.Register(c => new TestAuthenticationFilter(c.Resolve<ILogger>()))
                .AsWebApiAuthenticationFilterWhen(_controllerPredicate, FilterScope.Controller)
                .InstancePerRequest();

            builder.Register(c => new TestAuthorizationFilter(c.Resolve<ILogger>()))
                .AsWebApiAuthorizationFilterWhen(_controllerPredicate, FilterScope.Controller)
                .InstancePerRequest();

            builder.Register(c => new TestExceptionFilter(c.Resolve<ILogger>()))
                .AsWebApiExceptionFilterWhen(_controllerPredicate, FilterScope.Controller)
                .InstancePerRequest();

            builder.Register(c => new TestActionFilter(c.Resolve<ILogger>()))
                .AsWebApiActionFilterWhen(_controllerPredicate, FilterScope.Controller)
                .InstancePerRequest();

            var container = builder.Build();
            var provider = new CustomAutofacWebApiFilterProvider(container);
            var configuration = new HttpConfiguration
            {
                DependencyResolver = new Autofac.Integration.WebApi.AutofacWebApiDependencyResolver(container)
            };
            var actionDescriptor = BuildActionDescriptorForGetMethod();

            var filterInfos = provider.GetFilters(configuration, actionDescriptor).ToArray();
            var filters = filterInfos.Select(info => info.Instance).ToArray();

            Assert.That(filters, Has.Exactly(1).TypeOf<AuthenticationFilterWrapper>());
            Assert.That(filters, Has.Exactly(1).TypeOf<AuthorizationFilterWrapper>());
            Assert.That(filters, Has.Exactly(1).TypeOf<ExceptionFilterWrapper>());
            Assert.That(filters, Has.Exactly(1).TypeOf<ActionFilterWrapper>());
        }

        private static ReflectedHttpActionDescriptor BuildActionDescriptorForGetMethod()
        {
            return BuildActionDescriptorForGetMethod(typeof(TestController));
        }

        private static ReflectedHttpActionDescriptor BuildActionDescriptorForGetMethod(Type controllerType)
        {
            var controllerDescriptor = new HttpControllerDescriptor { ControllerType = controllerType };
            var methodInfo = typeof(TestController).GetMethod("Get");
            var actionDescriptor = new ReflectedHttpActionDescriptor(controllerDescriptor, methodInfo);
            return actionDescriptor;
        }
    }
}
