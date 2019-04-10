using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using FGS.Pump.Configuration.Patterns;
using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.Extensions;
using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.Pump.Configuration.Tests.Patterns
{
    [TestFixture]
    [Unit]
    public class EnumerableAdaptingAppSettingsTests : BaseUnitTest
    {
        [Test]
        public void GetEnumerator_DelegatesToAdapted()
        {
            var mockAdaptedEnumerator = Fixture.Mock<IEnumerator<KeyValuePair<string, string>>>();
            var expected = mockAdaptedEnumerator.Object;
            var mockAdapted = Fixture.Mock<IEnumerable<KeyValuePair<string, string>>>();
            mockAdapted.Setup(a => a.GetEnumerator()).Returns(expected);
            var subject = new EnumerableAdaptingAppSettings(mockAdapted.Object, StringComparer.OrdinalIgnoreCase);

            var actual = subject.GetEnumerator();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Indexer_ParameterizedWithString_ReturnsMatchFromAdapted()
        {
            var adapted = Fixture.CreateMany<KeyValuePair<string, string>>();
            var expectedKvp = adapted.Last();
            var expected = expectedKvp.Value;
            var appSettingName = expectedKvp.Key;
            var subject = new EnumerableAdaptingAppSettings(adapted, StringComparer.OrdinalIgnoreCase);

            var actual = subject[appSettingName];

            Assert.AreSame(expected, actual);
        }
    }
}
