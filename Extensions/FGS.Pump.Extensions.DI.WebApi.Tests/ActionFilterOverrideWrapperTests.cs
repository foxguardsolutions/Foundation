using System.Web.Http.Filters;

using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    [Unit]
    [TestFixture]
    public class ActionFilterOverrideWrapperTests
    {
        [Test]
        public void MetadataKeyReturnsOverrideValue()
        {
            var wrapper = new ActionFilterOverrideWrapper(new CustomWebApiFilterMetadata());
            Assert.That(wrapper.MetadataKey, Is.EqualTo(CustomAutofacWebApiFilterProvider.ActionFilterOverrideMetadataKey));
        }

        [Test]
        public void FiltersToOverrideReturnsCorrectType()
        {
            var wrapper = new ActionFilterOverrideWrapper(new CustomWebApiFilterMetadata());
            Assert.That(wrapper.FiltersToOverride, Is.EqualTo(typeof(IActionFilter)));
        }
    }
}
