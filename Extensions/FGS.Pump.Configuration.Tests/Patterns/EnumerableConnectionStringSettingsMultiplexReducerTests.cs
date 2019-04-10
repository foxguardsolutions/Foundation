using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class EnumerableConnectionStringSettingsMultiplexReducerTests : BaseUnitTest
    {
        private IEnumerable<IEnumerable<ConnectionStringSettings>> _adapted;
        private Lazy<EnumerableConnectionStringSettingsMultiplexReducer> _lazySubject;

        private EnumerableConnectionStringSettingsMultiplexReducer Subject => _lazySubject.Value;

        [SetUp]
        public void SetUp()
        {
            _lazySubject = new Lazy<EnumerableConnectionStringSettingsMultiplexReducer>(() => new EnumerableConnectionStringSettingsMultiplexReducer(_adapted.ToArray(), StringComparer.OrdinalIgnoreCase));
            Given_NoSources();
        }

        [Test]
        public void Enumerated_GivenNoSources_ReturnsEmpty()
        {
            Given_NoSources();

            var actual = Subject.ToArray();

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Enumerated_GivenSingleEmptySources_ReturnsEmpty()
        {
            Given_SingleEmptySource();

            var actual = Subject.ToArray();

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Enumerated_GivenManyEmptySources_ReturnsEmpty()
        {
            Given_ManyEmptySources();

            var actual = Subject.ToArray();

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Enumerated_GivenSingleSourceWithSingleItem_ReturnsItem()
        {
            var expected = Fixture.Create<ConnectionStringSettings>();
            Given_SingleSourceContaining(expected);

            var actual = Subject.ToArray();

            Assert.That(actual, Has.Length.EqualTo(1));
            Assert.That(actual, Has.One.SameAs(expected));
        }

        [Test]
        public void Enumerated_GivenManySourcesContainingManyDifferentItems_ReturnsAllItems()
        {
            var firstSet = Fixture.CreateMany<ConnectionStringSettings>().ToArray();
            Given_HasSourceContaining(firstSet);
            var secondSet = Fixture.CreateMany<ConnectionStringSettings>().ToArray();
            Given_HasSourceContaining(secondSet);
            var thirdSet = Fixture.CreateMany<ConnectionStringSettings>().ToArray();
            Given_HasSourceContaining(thirdSet);
            var expected = firstSet.Concat(secondSet).Concat(thirdSet).ToArray();

            var actual = Subject.ToArray();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Enumerated_GivenManySourcesContainingMatchingSingleItem_ReturnsFirstMatchingItem()
        {
            var similarItemBuilder = Fixture.Build<ConnectionStringSettings>().With(css => css.Name, Fixture.Create("Name"));
            var expected = similarItemBuilder.Create();
            Given_HasSourceContaining(expected);
            Given_HasSourceContaining(similarItemBuilder.Create());
            Given_HasSourceContaining(similarItemBuilder.Create());

            var actual = Subject.ToArray();

            Assert.That(actual, Has.Length.EqualTo(1));
            Assert.That(actual, Has.One.SameAs(expected));
        }

        private void Given_NoSources()
        {
            _adapted = Enumerable.Empty<IEnumerable<ConnectionStringSettings>>();
        }

        private void Given_SingleEmptySource()
        {
            Given_NoSources();
            Given_HasSourceContaining(Enumerable.Empty<ConnectionStringSettings>().ToArray());
        }

        private void Given_SingleSourceContaining(params ConnectionStringSettings[] content)
        {
            Given_NoSources();
            Given_HasSourceContaining(content);
        }

        private void Given_HasSourceContaining(params ConnectionStringSettings[] content)
        {
            _adapted = _adapted.Concat(new[] { content });
        }

        private void Given_ManyEmptySources()
        {
            _adapted = Enumerable.Range(0, Fixture.CreateSmallPositiveRandomNumber()).Select(x => Enumerable.Empty<ConnectionStringSettings>());
        }
    }
}
