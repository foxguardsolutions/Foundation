using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using AutoFixture;

using FGS.Pump.Configuration.Abstractions;
using FGS.Pump.Configuration.Patterns;
using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.Extensions;
using FGS.Pump.Tests.Support.TestCategories;

using Moq;

using NUnit.Framework;

namespace FGS.Pump.Configuration.Tests.Patterns
{
    [TestFixture]
    [Unit]
    public class ConnectionStringsAdaptingEnumerableTests : BaseUnitTest
    {
        private IEnumerable<ConnectionStringSettings> _adaptedContents;
        private Mock<IConnectionStrings> _mockAdapted;
        private Lazy<ConnectionStringsAdaptingEnumerable> _lazySubject;

        private ConnectionStringsAdaptingEnumerable Subject => _lazySubject.Value;

        [SetUp]
        public void SetUp()
        {
            _mockAdapted = Fixture.Mock<IConnectionStrings>();
            _mockAdapted.As<IEnumerable<KeyValuePair<string, ConnectionStringSettings>>>().Setup(x => x.GetEnumerator()).Returns(() => _adaptedContents.Select(x => new KeyValuePair<string, ConnectionStringSettings>(x.Name, x)).GetEnumerator());
            _lazySubject = new Lazy<ConnectionStringsAdaptingEnumerable>(() => new ConnectionStringsAdaptingEnumerable(_mockAdapted.Object));
        }

        [Test]
        public void Enumerated_ReturnsMatchFromAdapted()
        {
            var expected = Fixture.CreateMany<ConnectionStringSettings>();
            _adaptedContents = expected;

            var actual = Subject.ToArray();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
