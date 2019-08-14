using System;

using AutoFixture;
using AutoFixture.Dsl;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

using Moq;

namespace FGS.Tests.Support.AspNetCore.Mvc
{
    public static class AutoFixtureExtensions
    {
        public static IPostprocessComposer<TController> BuildController<TController>(this Fixture fixture, Action<Mock<HttpContext>> httpContextAdditionalSetup = null)
            where TController : Controller
        {
            var tempDataDictionary = new MockTempDataDictionary();
            fixture.Inject<ITempDataDictionary>(tempDataDictionary);

            return BuildApiController<TController>(fixture, httpContextAdditionalSetup)
                .With(c => c.TempData, tempDataDictionary)
                .Without(c => c.ViewData);
        }

        public static TController CreateController<TController>(this Fixture fixture, Action<Mock<HttpContext>> httpContextAdditionalSetup = null)
            where TController : Controller
        {
            return BuildController<TController>(fixture, httpContextAdditionalSetup).Create();
        }

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

        public static TApiController CreateApiController<TApiController>(this Fixture fixture, Action<Mock<HttpContext>> httpContextAdditionalSetup = null)
            where TApiController : ControllerBase
        {
            return BuildApiController<TApiController>(fixture, httpContextAdditionalSetup).Create();
        }

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
