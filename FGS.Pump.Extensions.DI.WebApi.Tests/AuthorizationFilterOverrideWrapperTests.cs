﻿using System.Web.Http.Filters;

using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/AuthorizationFilterOverrideWrapperFixture.cs </remarks>
    [Unit]
    [TestFixture]
    public class AuthorizationFilterOverrideWrapperTests
    {
        [Test]
        public void MetadataKeyReturnsOverrideValue()
        {
            var wrapper = new AuthorizationFilterOverrideWrapper(new CustomWebApiFilterMetadata());
            Assert.That(wrapper.MetadataKey, Is.EqualTo(CustomAutofacWebApiFilterProvider.AuthorizationFilterOverrideMetadataKey));
        }

        [Test]
        public void FiltersToOverrideReturnsCorrectType()
        {
            var wrapper = new AuthorizationFilterOverrideWrapper(new CustomWebApiFilterMetadata());
            Assert.That(wrapper.FiltersToOverride, Is.EqualTo(typeof(IAuthorizationFilter)));
        }
    }
}
