using System;
using System.Configuration;
using System.Linq;

using AutoFixture;

using FGS.Pump.Configuration.Patterns;
using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.Pump.Configuration.Tests.Patterns
{
    [TestFixture]
    [Unit]
    public class EnumerableAdaptingConnectionStringsTests : BaseUnitTest
    {
        [Test]
        public void Indexer_ParameterizedWithString_ReturnsMatchFromAdapted()
        {
            var adapted = Fixture.CreateMany<ConnectionStringSettings>();
            var expected = adapted.Last();
            var connectionStringName = expected.Name;
            var subject = new EnumerableAdaptingConnectionStrings(adapted, StringComparer.OrdinalIgnoreCase);

            var actual = subject[connectionStringName];

            Assert.AreSame(expected, actual);
        }
    }
}
