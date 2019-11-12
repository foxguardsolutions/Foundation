using System;

using AutoFixture;
using AutoFixture.Dsl;

using FGS.Tests.Support.AutoFixture.Mocking;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

using Moq;

namespace FGS.Tests.Support.AspNetCore.Mvc
{
    /// <summary>
    /// Extends <see cref="Fixture"/> with functionality to create instances of types helpful for testing code dependent on ASP.NET Core.
    /// </summary>
    public static class AutoFixtureExtensions
    {
        /// <summary>
        /// Creates a <see cref="IPostprocessComposer{TController}"/> that can be used to create instances of <typeparamref name="TController"/>.
        /// </summary>
        /// <typeparam name="TController">The type of controller to create a composer for.</typeparam>
        /// <param name="fixture">The <see cref="Fixture"/> used to generate values.</param>
        /// <param name="httpContextAdditionalSetup">An optional action that defines additional logic for configuring the <see cref="Mock{HttpContext}"/> that will be used in composed object graphs.</param>
        /// <returns>A <see cref="IPostprocessComposer{TController}"/> that can be used to create instances of <typeparamref name="TController"/>.</returns>
        public static IPostprocessComposer<TController> BuildController<TController>(this Fixture fixture, Action<Mock<HttpContext>> httpContextAdditionalSetup = null)
            where TController : Controller
        {
            var tempDataDictionary = new MockTempDataDictionary();
            fixture.Inject<ITempDataDictionary>(tempDataDictionary);

            return BuildApiController<TController>(fixture, httpContextAdditionalSetup)
                .With(c => c.TempData, tempDataDictionary)
                .Without(c => c.ViewData);
        }

        /// <summary>
        /// Creates a controller of type <typeparamref name="TController"/>.
        /// </summary>
        /// <typeparam name="TController">The type of controller to create.</typeparam>
        /// <param name="fixture">The <see cref="Fixture"/> used to generate values.</param>
        /// <param name="httpContextAdditionalSetup">An optional action that defines additional logic for configuring the <see cref="Mock{HttpContext}"/> that will be used in composed object graphs.</param>
        /// <returns>An instance of <typeparamref name="TController"/>.</returns>
        public static TController CreateController<TController>(this Fixture fixture, Action<Mock<HttpContext>> httpContextAdditionalSetup = null)
            where TController : Controller
        {
            return BuildController<TController>(fixture, httpContextAdditionalSetup).Create();
        }

        /// <summary>
        /// Creates a <see cref="IPostprocessComposer{TApiController}"/> that can be used to create instances of <typeparamref name="TApiController"/>.
        /// </summary>
        /// <typeparam name="TApiController">The type of API controller to create a composer for.</typeparam>
        /// <param name="fixture">The <see cref="Fixture"/> used to generate values.</param>
        /// <param name="httpContextAdditionalSetup">An optional action that defines additional logic for configuring the <see cref="Mock{HttpContext}"/> that will be used in composed object graphs.</param>
        /// <returns>A <see cref="IPostprocessComposer{TApiController}"/> that can be used to create instances of <typeparamref name="TApiController"/>.</returns>
        public static IPostprocessComposer<TApiController> BuildApiController<TApiController>(this Fixture fixture, Action<Mock<HttpContext>> httpContextAdditionalSetup = null)
            where TApiController : ControllerBase
        {
            var mockUrlHelper = fixture.MockUrlHelper();

            var mockHttpContext = fixture.Mock<HttpContext>();
            httpContextAdditionalSetup?.Invoke(mockHttpContext);

            var lazyControllerContext = new Lazy<ControllerContext>(() => new ControllerContext(new ActionContext(mockHttpContext.Object, new RouteData(), new ControllerActionDescriptor())));

            return fixture.Build<TApiController>()
                .Without(c => c.ControllerContext)
                .Do(c => c.ControllerContext = lazyControllerContext.Value)
                .With(c => c.Url, mockUrlHelper.Object);
        }

        /// <summary>
        /// Creates an API controller of type <typeparamref name="TApiController"/>.
        /// </summary>
        /// <typeparam name="TApiController">The type of API controller to create.</typeparam>
        /// <param name="fixture">The <see cref="Fixture"/> used to generate values.</param>
        /// <param name="httpContextAdditionalSetup">An optional action that defines additional logic for configuring the <see cref="Mock{HttpContext}"/> that will be used in composed object graphs.</param>
        /// <returns>An instance of <typeparamref name="TApiController"/>.</returns>
        public static TApiController CreateApiController<TApiController>(this Fixture fixture, Action<Mock<HttpContext>> httpContextAdditionalSetup = null)
            where TApiController : ControllerBase
        {
            return BuildApiController<TApiController>(fixture, httpContextAdditionalSetup).Create();
        }

        /// <summary>
        /// Creates and registers a mocked <see cref="IUrlHelper"/> with the <paramref name="fixture"/>, and returns it for further configuration.
        /// </summary>
        /// <param name="fixture">The <see cref="Fixture"/> into which the mock is registered.</param>
        /// <returns>A <see cref="Mock{IUrlHelper}"/>.</returns>
        public static Mock<IUrlHelper> MockUrlHelper(this Fixture fixture)
        {
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(uh => uh.Action(It.IsAny<UrlActionContext>())).Returns(fixture.Create<string>);
            mockUrlHelper.Setup(uh => uh.Content(It.IsAny<string>())).Returns(fixture.Create<string>);
            mockUrlHelper.Setup(uh => uh.IsLocalUrl(It.IsAny<string>())).Returns(fixture.Create<bool>);
            mockUrlHelper.Setup(uh => uh.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(fixture.Create<string>);
            mockUrlHelper.Setup(uh => uh.RouteUrl(It.IsAny<UrlRouteContext>())).Returns(fixture.Create<string>);
            return mockUrlHelper;
        }
    }
}
