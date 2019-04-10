using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using AutoFixture;

using FGS.Pump.Configuration.Environment;
using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FGS.Pump.Configuration.Tests.Environment
{
    [TestFixture]
    [Unit]
    public class SplitConnectionStringDictionaryAdaptingConnectionStringEnumerableTests : BaseUnitTest
    {
        private IDictionary<string, string> _adapted;
        private ISplitConnectionStringAdaptationStrategy _adaptationStrategy;
        private StringComparer _connectionStringNameComparer;
        private Lazy<SplitConnectionStringDictionaryAdaptingConnectionStringEnumerable> _lazySubject;
        private SplitConnectionStringDictionaryAdaptingConnectionStringEnumerable Subject => _lazySubject.Value;

        [SetUp]
        public void SetUp()
        {
            Given_EmptyAdapted();
            _adaptationStrategy = new EnvironmentKeySplitConnectionStringAdaptationStrategy();
            _connectionStringNameComparer = StringComparer.OrdinalIgnoreCase;
            _lazySubject =
                new Lazy<SplitConnectionStringDictionaryAdaptingConnectionStringEnumerable>(() => new SplitConnectionStringDictionaryAdaptingConnectionStringEnumerable(_adapted, _adaptationStrategy, _connectionStringNameComparer));
        }

        [Test]
        public void GetEnumerator_GivenEmptyAdapted_ReturnsEmpty()
        {
            Given_EmptyAdapted();
            var expected = Enumerable.Empty<ConnectionStringSettings>();

            var actual = Subject.ToList();

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void GetEnumerator_GivenSingleNonMatchingInAdapted_ReturnsEmpty()
        {
            Given_SingleNonMatchingInAdapted();

            var actual = Subject.ToList();

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetEnumerator_GivenSingleMatchingConnectionStringValueInAdapted_ReturnsSingleExpectedConnectionString()
        {
            var expected = Given_SingleMatchingConnectionStringValueInAdapted();

            var actual = Subject.ToList().Single();

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void GetEnumerator_GivenMatchingConnectionStringValueAndNonMatchingInAdapted_ReturnsSingleExpectedConnectionString()
        {
            Given_SingleNonMatchingInAdapted();
            var expected = Given_ConnectionStringWithOnlyValueIncludedInAdapted();

            var actual = Subject.ToList().Single();

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void GetEnumerator_GivenMatchingConnectionStringValueAndProviderInAdapted_ReturnsSingleExpectedConnectionString()
        {
            var expected = Given_ConnectionStringWithValueAndProviderIncludedInAdapted();

            var actual = Subject.ToList().Single();

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void GetEnumerator_GivenMatchingConnectionStringValueAndProviderAndNonMatchingInAdapted_ReturnsSingleExpectedConnectionString()
        {
            Given_SingleNonMatchingInAdapted();
            var expected = Given_ConnectionStringWithValueAndProviderIncludedInAdapted();

            var actual = Subject.ToList().Single();

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void GetEnumerator_GivenMatchingConnectionStringProviderInAdapted_ReturnsEmpty()
        {
            Given_ConnectionStringProviderIncludedInAdapted();

            var actual = Subject.ToList();

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetEnumerator_GivenTwoMatchingConnectionStringValuesAndProvidersInAdapted_ReturnsBothExpectedConnectionString()
        {
            var expecteds = new[] { Given_ConnectionStringWithValueAndProviderIncludedInAdapted(), Given_ConnectionStringWithValueAndProviderIncludedInAdapted() };

            var actuals = Subject.ToArray();

            Assert_ActualConnectionStringsEquivalentToExpecteds(actuals, expecteds);
        }

        private void Assert_ActualConnectionStringsEquivalentToExpecteds(ConnectionStringSettings[] actuals, ConnectionStringSettings[] expecteds)
        {
            Assert.That(actuals, Has.Length.EqualTo(expecteds.Length));
            foreach (var expected in expecteds)
            {
                Assert_ActualConnectionStringsContainExpected(actuals, expected);
            }
        }

        private void Assert_ActualConnectionStringsContainExpected(IEnumerable<ConnectionStringSettings> actuals, ConnectionStringSettings expected)
        {
            Assert.That(actuals, AssertionConstraintForMatching(Has.One, expected));
        }

        private void Assert_ActualConnectionStringEqualsExpected(ConnectionStringSettings actual, ConnectionStringSettings expected)
        {
            Assert.That(actual, AssertionConstraintForMatching(new ConstraintExpression(), expected));
        }

        private Constraint AssertionConstraintForMatching(ConstraintExpression leftHas, ConnectionStringSettings expected)
        {
            return
                leftHas.Property(nameof(ConnectionStringSettings.Name)).EqualTo(expected.Name)
                .And.Property(nameof(ConnectionStringSettings.ConnectionString)).EqualTo(expected.ConnectionString)
                .And.Property(nameof(ConnectionStringSettings.ProviderName)).EqualTo(expected.ProviderName);
        }

        private void Given_ConnectionStringProviderIncludedInAdapted() =>
            Given_ConnectionStringProviderIncludedInAdapted(Fixture.Create<string>(), Fixture.Create<string>());

        private ConnectionStringSettings Given_SingleMatchingConnectionStringValueInAdapted()
        {
            Given_EmptyAdapted();
            return Given_ConnectionStringWithOnlyValueIncludedInAdapted();
        }

        private void Given_SingleNonMatchingInAdapted()
        {
            Given_EmptyAdapted();
            Given_KeyValuePairIncludedInAdapted(Fixture.Create<string>(), Fixture.Create<string>());
        }

        private ConnectionStringSettings Given_ConnectionStringWithValueAndProviderIncludedInAdapted()
        {
            var connectionString = new ConnectionStringSettings(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>());
            Given_ConnectionStringIncludedInAdapted(connectionString);
            return connectionString;
        }

        private ConnectionStringSettings Given_ConnectionStringWithOnlyValueIncludedInAdapted()
        {
            var connectionString = new ConnectionStringSettings(Fixture.Create<string>(), Fixture.Create<string>());
            Given_ConnectionStringIncludedInAdapted(connectionString);
            return connectionString;
        }

        private void Given_ConnectionStringIncludedInAdapted(ConnectionStringSettings connectionString)
        {
            Given_ConnectionStringValueIncludedInAdapted(connectionString.Name, connectionString.ConnectionString);
            if (!string.IsNullOrWhiteSpace(connectionString.ProviderName))
            {
                Given_ConnectionStringProviderIncludedInAdapted(connectionString.Name, connectionString.ProviderName);
            }
        }

        private void Given_ConnectionStringValueIncludedInAdapted(string connectionStringName, string value)
        {
            const string Separator = EnvironmentKeySplitConnectionStringAdaptationStrategy.Separator;
            const string Prefix = EnvironmentKeySplitConnectionStringAdaptationStrategy.Prefix;
            const string ValueSuffix = EnvironmentKeySplitConnectionStringAdaptationStrategy.ValueSuffix;

            Given_KeyValuePairIncludedInAdapted(Prefix + Separator + connectionStringName + Separator + ValueSuffix, value);
        }

        private void Given_ConnectionStringProviderIncludedInAdapted(string connectionStringName, string provider)
        {
            const string Separator = EnvironmentKeySplitConnectionStringAdaptationStrategy.Separator;
            const string Prefix = EnvironmentKeySplitConnectionStringAdaptationStrategy.Prefix;
            const string ProviderSuffix = EnvironmentKeySplitConnectionStringAdaptationStrategy.ProviderSuffix;

            Given_KeyValuePairIncludedInAdapted(Prefix + Separator + connectionStringName + Separator + ProviderSuffix, provider);
        }

        private void Given_KeyValuePairIncludedInAdapted(string key, string value)
        {
            Given_KeyValuePairsIncludedInAdapted(new[] { new KeyValuePair<string, string>(key, value) });
        }

        private void Given_KeyValuePairsIncludedInAdapted(IEnumerable<KeyValuePair<string, string>> kvps)
        {
            _adapted = _adapted.Union(kvps).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private void Given_EmptyAdapted()
        {
            _adapted = new Dictionary<string, string>();
        }
    }
}
