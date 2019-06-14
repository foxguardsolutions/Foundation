using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

using Autofac;
using Autofac.Integration.Mvc;

using FGS.Pump.Extensions.DI.Mvc.Tests.TestTypes;

using Moq;

using NUnit.Framework;

namespace FGS.Pump.Extensions.DI.Mvc.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/e26ce3fe9ccc639f1349bcd8aee8e6e4ee066346/test/Autofac.Integration.Mvc.Test/AutofacFilterProviderFixture.cs </remarks>
    [TestFixture]
    public class CustomAutofacFilterProviderTests
    {
        private ControllerContext _baseControllerContext;
        private ControllerDescriptor _controllerDescriptor;

        private MethodInfo _baseMethodInfo;
        private string _actionName;

        private ReflectedActionDescriptor _reflectedActionDescriptor;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            _baseControllerContext = new ControllerContext { Controller = new TestController() };

            _baseMethodInfo = TestController.GetAction1MethodInfo<TestController>();
            _actionName = _baseMethodInfo.Name;

            _controllerDescriptor = new Mock<ControllerDescriptor>().Object;
            _reflectedActionDescriptor = new ReflectedActionDescriptor(_baseMethodInfo, _actionName, _controllerDescriptor);
        }

        [Test]
        public void FilterRegistrationsWithoutMetadataIgnored()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AuthorizeAttribute>().AsImplementedInterfaces();
            var container = builder.Build();
            SetupMockLifetimeScopeProvider(container);
            var provider = new CustomAutofacFilterProvider();

            var filters = provider.GetFilters(_baseControllerContext, _reflectedActionDescriptor).ToList();
            Assert.That(filters, Has.Count.EqualTo(0));
        }

        [Test]
        public void CanRegisterMultipleFilterTypesAgainstSingleService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new TestCombinationFilter())
                .AsActionFilterWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: 0)
                .AsAuthenticationFilterWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: 0)
                .AsAuthorizationFilterWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: 0)
                .AsExceptionFilterWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: 0)
                .AsResultFilterWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: 0);
            var container = builder.Build();

            Assert.That(container.Resolve<IActionFilter>(), Is.Not.Null);
            Assert.That(container.Resolve<IAuthenticationFilter>(), Is.Not.Null);
            Assert.That(container.Resolve<IAuthorizationFilter>(), Is.Not.Null);
            Assert.That(container.Resolve<IExceptionFilter>(), Is.Not.Null);
            Assert.That(container.Resolve<IResultFilter>(), Is.Not.Null);
        }

        private static void SetupMockLifetimeScopeProvider(ILifetimeScope container)
        {
            var resolver = new AutofacDependencyResolver(container, new StubLifetimeScopeProvider(container));
            DependencyResolver.SetResolver(resolver);
        }
    }
}
