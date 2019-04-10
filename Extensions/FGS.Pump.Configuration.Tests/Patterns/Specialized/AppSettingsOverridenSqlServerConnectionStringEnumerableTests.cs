using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using AutoFixture;
using AutoFixture.Kernel;

using FGS.Pump.Configuration.Abstractions;
using FGS.Pump.Configuration.Patterns.Specialized;
using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.Extensions;
using FGS.Pump.Tests.Support.TestCategories;

using Moq;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FGS.Pump.Configuration.Tests.Patterns.Specialized
{
    [TestFixture]
    [Unit]
    public class AppSettingsOverridenSqlServerConnectionStringEnumerableTests : BaseUnitTest
    {
        private IEnumerable<ConnectionStringSettings> _adapted;
        private IDictionary<string, string> _overrideSourceMockSource;
        private Mock<IAppSettings> _mockOverrideSource;
        private StringComparer _connectionStringNameComparer;
        private StringComparer _connectionStringPartNameComparer;
        private StringComparer _connectionStringProviderComparer;
        private Lazy<AppSettingsOverridenSqlServerConnectionStringEnumerable> _lazySubject;
        private AppSettingsOverridenSqlServerConnectionStringEnumerable Subject => _lazySubject.Value;

        [SetUp]
        public void SetUp()
        {
            Given_EmptyAdapted();
            Given_EmptyOverrides();
            var adaptationStrategy = new AppSettingsOverriddenSqlServerConnectionStringsAdaptationStrategy();
            _connectionStringNameComparer = _connectionStringPartNameComparer = _connectionStringProviderComparer = StringComparer.OrdinalIgnoreCase;
            var sqlServerConnectionStringBuilderApplicator = new SqlServerConnectionStringBuilderApplicator();
            _lazySubject =
                new Lazy<AppSettingsOverridenSqlServerConnectionStringEnumerable>(() =>
                    new AppSettingsOverridenSqlServerConnectionStringEnumerable(
                        _adapted,
                        _mockOverrideSource.Object,
                        adaptationStrategy,
                        _connectionStringNameComparer,
                        _connectionStringPartNameComparer,
                        _connectionStringProviderComparer,
                        sqlServerConnectionStringBuilderApplicator));
        }

        [Test]
        public void GetEnumerator_GivenEmptyAdaptedAndEmptyOverrides_ReturnsEmpty()
        {
            Given_EmptyAdapted();
            Given_EmptyOverrides();
            var expected = Enumerable.Empty<ConnectionStringSettings>();

            var actual = Subject.ToList();

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void GetEnumerator_GivenSingleConnectionStringForSqlServerInAdaptedAndEmptyOverrides_ReturnsExpected()
        {
            var expected = Given_ConnectionStringForSqlServerIncludedInAdapted();
            Given_EmptyOverrides();

            var actual = Subject.Single();

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void GetEnumerator_GivenSingleConnectionStringForDefaultInAdaptedAndEmptyOverrides_ReturnsExpected()
        {
            var expected = Given_ConnectionStringForDefaultIncludedInAdapted();
            Given_EmptyOverrides();

            var actual = Subject.Single();

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void GetEnumerator_GivenSingleConnectionStringForUnknownInAdaptedAndEmptyOverrides_ReturnsExpected()
        {
            var expected = Given_ConnectionStringForUnknownIncludedInAdapted();
            Given_EmptyOverrides();

            var actual = Subject.Single();

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        [TestCaseSource(nameof(SingleFieldsBeingOverridden))]
        public void GetEnumerator_GivenSingleConnectionStringForSqlServerInAdaptedAndSingleOverrides_ReturnsExpected(Expression memberSelector)
        {
            var connectionStringSettings = Given_ConnectionStringForSqlServerIncludedInAdapted();
            var member = ((memberSelector as LambdaExpression).Body as MemberExpression).Member;
            var expectedValue = FixtureCreateForMember(member);
            Given_OverridesIncludes(CreateOverrideAppSettingName(connectionStringSettings.Name, member.Name), expectedValue.ToString());

            var actualConnectionStringSettings = Subject.Single();

            var builderFromActualConnectionStringSettings = new SqlConnectionStringBuilder(actualConnectionStringSettings.ConnectionString);
            var actualValue = GetConnectionStringBuilderMemberValue(memberSelector, builderFromActualConnectionStringSettings);
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCaseSource(nameof(SingleFieldsBeingOverridden))]
        public void GetEnumerator_GivenSingleConnectionStringForDefaultInAdaptedAndSingleOverride_ReturnsExpected(Expression memberSelector)
        {
            var connectionStringSettings = Given_ConnectionStringForDefaultIncludedInAdapted();
            var member = ((memberSelector as LambdaExpression).Body as MemberExpression).Member;
            var expectedValue = FixtureCreateForMember(member);
            Given_OverridesIncludes(CreateOverrideAppSettingName(connectionStringSettings.Name, member.Name), expectedValue.ToString());

            var actualConnectionStringSettings = Subject.Single();

            var builderFromActualConnectionStringSettings = new SqlConnectionStringBuilder(actualConnectionStringSettings.ConnectionString);
            var actualValue = GetConnectionStringBuilderMemberValue(memberSelector, builderFromActualConnectionStringSettings);
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCaseSource(nameof(ManyFieldsBeingOverridden))]
        public void GetEnumerator_GivenSingleConnectionStringForSqlServerInAdaptedAndManyOverrides_ReturnsExpected(params Expression[] memberSelectors)
        {
            var connectionStringSettings = Given_ConnectionStringForSqlServerIncludedInAdapted();
            var membersByMemberSelector = memberSelectors.ToDictionary(ms => ms, ms => ((ms as LambdaExpression).Body as MemberExpression).Member);
            var expectedValuesByMemberSelector = memberSelectors.ToDictionary(ms => ms, ms => FixtureCreateForMember(membersByMemberSelector[ms]));
            var overrides = expectedValuesByMemberSelector.ToDictionary(kvp => CreateOverrideAppSettingName(connectionStringSettings.Name, membersByMemberSelector[kvp.Key].Name), kvp => kvp.Value.ToString()).ToList();
            overrides.ForEach(kvp => Given_OverridesIncludes(kvp.Key, kvp.Value));

            var actual = Subject.Single().ConnectionString;

            var actualAsConnectionStringBuilder = new SqlConnectionStringBuilder(actual);
            Assert_ThatActualConnectionStringBuilderMembersEqualExpected(actualAsConnectionStringBuilder, memberSelectors, expectedValuesByMemberSelector);
        }

        [Test]
        [TestCaseSource(nameof(ManyFieldsBeingOverridden))]
        public void GetEnumerator_GivenSingleConnectionStringForDefaultInAdaptedAndManyOverrides_ReturnsExpected(params Expression[] memberSelectors)
        {
            var connectionStringSettings = Given_ConnectionStringForDefaultIncludedInAdapted();
            var membersByMemberSelector = memberSelectors.ToDictionary(ms => ms, ms => ((ms as LambdaExpression).Body as MemberExpression).Member);
            var expectedValuesByMemberSelector = memberSelectors.ToDictionary(ms => ms, ms => FixtureCreateForMember(membersByMemberSelector[ms]));
            var overrides = expectedValuesByMemberSelector.ToDictionary(kvp => CreateOverrideAppSettingName(connectionStringSettings.Name, membersByMemberSelector[kvp.Key].Name), kvp => kvp.Value.ToString()).ToList();
            overrides.ForEach(kvp => Given_OverridesIncludes(kvp.Key, kvp.Value));

            var actual = Subject.Single().ConnectionString;

            var actualAsConnectionStringBuilder = new SqlConnectionStringBuilder(actual);
            Assert_ThatActualConnectionStringBuilderMembersEqualExpected(actualAsConnectionStringBuilder, memberSelectors, expectedValuesByMemberSelector);
        }

        private static IEnumerable<TestCaseData> ManyFieldsBeingOverridden()
        {
            var memberSelectors = SingleFieldsBeingOverridden().Select(x => x.Arguments.First()).Cast<Expression>().ToArray();
            var testCases = Enumerable.Range(1, memberSelectors.Length - 2).Select(i => new[] { memberSelectors[i - 1], memberSelectors[i], memberSelectors[i + 1] });

            return testCases.Select(tc => new TestCaseData(new object[] { tc }));
        }

        private static IEnumerable<TestCaseData> SingleFieldsBeingOverridden()
        {
            TestCaseData CreateTestCaseData<T>(Expression<Func<SqlConnectionStringBuilder, T>> selector) => new TestCaseData(selector);

            yield return CreateTestCaseData(csb => csb.ApplicationIntent);
            yield return CreateTestCaseData(csb => csb.ApplicationName);
            yield return CreateTestCaseData(csb => csb.AsynchronousProcessing);
            yield return CreateTestCaseData(csb => csb.AttachDBFilename);
            yield return CreateTestCaseData(csb => csb.Authentication);
            yield return CreateTestCaseData(csb => csb.ColumnEncryptionSetting);
            yield return CreateTestCaseData(csb => csb.ConnectRetryCount);
            yield return CreateTestCaseData(csb => csb.ConnectTimeout);
            yield return CreateTestCaseData(csb => csb.ContextConnection);
            yield return CreateTestCaseData(csb => csb.CurrentLanguage);
            yield return CreateTestCaseData(csb => csb.DataSource);
            yield return CreateTestCaseData(csb => csb.Encrypt);
            yield return CreateTestCaseData(csb => csb.Enlist);
            yield return CreateTestCaseData(csb => csb.FailoverPartner);
            yield return CreateTestCaseData(csb => csb.InitialCatalog);
            yield return CreateTestCaseData(csb => csb.LoadBalanceTimeout);
            yield return CreateTestCaseData(csb => csb.MaxPoolSize);
            yield return CreateTestCaseData(csb => csb.MinPoolSize);
            yield return CreateTestCaseData(csb => csb.MultipleActiveResultSets);
            yield return CreateTestCaseData(csb => csb.MultiSubnetFailover);
            yield return CreateTestCaseData(csb => csb.Password);
            yield return CreateTestCaseData(csb => csb.PersistSecurityInfo);
            yield return CreateTestCaseData(csb => csb.PoolBlockingPeriod);
            yield return CreateTestCaseData(csb => csb.Pooling);
            yield return CreateTestCaseData(csb => csb.Replication);
            yield return CreateTestCaseData(csb => csb.TransactionBinding);
            yield return CreateTestCaseData(csb => csb.TransparentNetworkIPResolution);
            yield return CreateTestCaseData(csb => csb.TrustServerCertificate);
            yield return CreateTestCaseData(csb => csb.TypeSystemVersion);
            yield return CreateTestCaseData(csb => csb.UserID);
            yield return CreateTestCaseData(csb => csb.UserInstance);
            yield return CreateTestCaseData(csb => csb.WorkstationID);
        }

        private static void Assert_ThatActualConnectionStringBuilderMembersEqualExpected(
            SqlConnectionStringBuilder builderFromActualConnectionStringSettings,
            IEnumerable<Expression> memberSelectors,
            IDictionary<Expression, object> expectedValuesByMemberSelector)
        {
            var membersByMemberSelector = memberSelectors.ToDictionary(ms => ms, ms => ((ms as LambdaExpression).Body as MemberExpression).Member);

            var assertionConstraint = memberSelectors.Skip(1)
                .Aggregate(
                    Has.Property(membersByMemberSelector[memberSelectors.First()].Name).EqualTo(expectedValuesByMemberSelector[memberSelectors.First()]),
                    (p, c) => p.And.Property(membersByMemberSelector[c].Name).EqualTo(expectedValuesByMemberSelector[c]));

            Assert.That(builderFromActualConnectionStringSettings, assertionConstraint);
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

        private void Given_OverridesIncludes(string appSettingName, string appSettingValue)
        {
            _overrideSourceMockSource.Add(appSettingName, appSettingValue);
        }

        private ConnectionStringSettings Given_ConnectionStringForUnknownIncludedInAdapted()
        {
            var connectionString = new ConnectionStringSettings(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>());
            Given_ConnectionStringIncludedInAdapted(connectionString);
            return connectionString;
        }

        private ConnectionStringSettings Given_ConnectionStringForDefaultIncludedInAdapted()
        {
            var connectionString = new ConnectionStringSettings(Fixture.Create<string>(), CreateRandomSqlServerConnectionStringValue());
            Given_ConnectionStringIncludedInAdapted(connectionString);
            return connectionString;
        }

        private ConnectionStringSettings Given_ConnectionStringForSqlServerIncludedInAdapted()
        {
            var connectionString = new ConnectionStringSettings(Fixture.Create<string>(), CreateRandomSqlServerConnectionStringValue(), AppSettingsOverridenSqlServerConnectionStringEnumerable.ExpectedOrDefaultProvider);
            Given_ConnectionStringIncludedInAdapted(connectionString);
            return connectionString;
        }

        private static object GetConnectionStringBuilderMemberValue(Expression memberSelector, SqlConnectionStringBuilder connectionStringBuilder)
        {
            var originalLambda = memberSelector as LambdaExpression;
            var memberExpression = originalLambda.Body as MemberExpression;
            var castExpression = Expression.Convert(memberExpression, typeof(object));
            var newLambda = Expression.Lambda<Func<SqlConnectionStringBuilder, object>>(castExpression, originalLambda.Parameters);
            return newLambda.Compile()(connectionStringBuilder);
        }

        private object FixtureCreateForMember(MemberInfo member) => new SpecimenContext(Fixture).Resolve(new SeededRequest((member as PropertyInfo).PropertyType, null));

        private string CreateOverrideAppSettingName(string connectionStringName, string propertyName) => connectionStringName + "_ConnectionString_" + propertyName;

        private string CreateRandomSqlServerConnectionStringValue() =>
            Fixture.Build<SqlConnectionStringBuilder>()
                .Without(csb => csb.ConnectionString)
                .Without(csb => csb.ConnectRetryInterval)
                .Without(csb => csb.NetworkLibrary)
                .Without(csb => csb.PacketSize)
                .WithAutoProperties()
                .Create()
                .ConnectionString;

        private void Given_ConnectionStringIncludedInAdapted(ConnectionStringSettings connectionStringSetting)
        {
            _adapted = _adapted.Union(new[] { connectionStringSetting });
        }

        private void Given_EmptyOverrides()
        {
            _overrideSourceMockSource = new Dictionary<string, string>();
            _mockOverrideSource = Fixture.Mock<IAppSettings>();
            _mockOverrideSource.Setup(os => os.Keys).Returns(() => _overrideSourceMockSource.Keys);
            _mockOverrideSource.Setup(os => os[It.IsAny<string>()]).Returns((string key) => _overrideSourceMockSource[key]);
        }

        private void Given_EmptyAdapted()
        {
            _adapted = Enumerable.Empty<ConnectionStringSettings>();
        }
    }
}
