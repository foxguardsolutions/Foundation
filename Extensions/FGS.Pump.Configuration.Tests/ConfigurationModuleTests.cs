using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

using Autofac;

using AutoFixture;

using FGS.Pump.Configuration.Abstractions;
using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.Extensions;
using FGS.Pump.Tests.Support.TestCategories;

using Moq;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using IConfigurationManager = System.Configuration.Abstractions.IConfigurationManager;
using IConfigurationManagerAppSettings = System.Configuration.Abstractions.IAppSettings;
using IConfigurationManagerConnectionStrings = System.Configuration.Abstractions.IConnectionStrings;

namespace FGS.Pump.Configuration.Tests
{
    [TestFixture]
    [Unit]
    public class ConfigurationModuleTests : BaseUnitTest, IDisposable
    {
        private NameValueCollection _configurationManagerAppSettingsContent;
        private List<ConnectionStringSettings> _configurationManagerConnectionStringsContent;
        private Mock<IConfigurationManagerAppSettings> _mockConfigurationManagerAppSettings;
        private Mock<IConfigurationManagerConnectionStrings> _mockConfigurationManagerConnectionStrings;
        private Mock<IConfigurationManager> _mockConfigurationManager;
        private Lazy<IContainer> _lazyContainer;
        private Lazy<IAppSettings> _lazyAppSettings;
        private Lazy<IConnectionStrings> _lazyConnectionStrings;

        protected IDictionary<string, string> EnvironmentVariables { get; set; }
        protected IAppSettings AppSettings => _lazyAppSettings.Value;
        protected IConnectionStrings ConnectionStrings => _lazyConnectionStrings.Value;

        [SetUp]
        public void SetUp()
        {
            SetupMockConfigurationManager();
            EnvironmentVariables = Fixture.CreateMany<KeyValuePair<string, string>>().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new ConfigurationModuleUnderTest(() => _mockConfigurationManager.Object, () => EnvironmentVariables));
            _lazyContainer = new Lazy<IContainer>(() => containerBuilder.Build());
            _lazyAppSettings = new Lazy<IAppSettings>(() => _lazyContainer.Value.Resolve<IAppSettings>());
            _lazyConnectionStrings = new Lazy<IConnectionStrings>(() => _lazyContainer.Value.Resolve<IConnectionStrings>());
        }

        [Test]
        public void AppSettings_Resolve()
        {
            Assert.That(AppSettings, Is.Not.Null);
        }

        [Test]
        public void ConnectionStrings_Resolve()
        {
            Assert.That(ConnectionStrings, Is.Not.Null);
        }

        [Test]
        public void AppSettings_IndexerParameterizedWithString_GivenExistsInConfigManager_ReturnsExpected()
        {
            var appSettingName = Fixture.Create<string>();
            var expectedAppSettingValue = Fixture.Create<string>();
            Given_AppSettingExistsInConfigManager(appSettingName, expectedAppSettingValue);

            var actualAppSettingValue = AppSettings[appSettingName];

            Assert.That(actualAppSettingValue, Is.EqualTo(expectedAppSettingValue));
        }

        [Test]
        public void AppSettings_IndexerParameterizedWithString_GivenExistsInEnvironment_ReturnsExpected()
        {
            var appSettingName = Fixture.Create<string>();
            var expectedAppSettingValue = Fixture.Create<string>();
            Given_AppSettingExistsInEnvironment(appSettingName, expectedAppSettingValue);

            var actualAppSettingValue = AppSettings[appSettingName];

            Assert.That(actualAppSettingValue, Is.EqualTo(expectedAppSettingValue));
        }

        [Test]
        public void AppSettings_IndexerParameterizedWithString_GivenExistsInEnvironmentAndConfigManager_ReturnsMatchFromEnvironment()
        {
            var appSettingName = Fixture.Create<string>();
            var expectedAppSettingValue = Fixture.Create<string>();
            var badAppSettingValue = Fixture.Create<string>();
            Given_AppSettingExistsInEnvironment(appSettingName, expectedAppSettingValue);
            Given_AppSettingExistsInConfigManager(appSettingName, badAppSettingValue);

            var actualAppSettingValue = AppSettings[appSettingName];

            Assert.That(actualAppSettingValue, Is.EqualTo(expectedAppSettingValue));
        }

        [Test]
        public void AppSettings_IndexerParameterizedWithString_GivenNotExists_ReturnsNull()
        {
            var appSettingName = Fixture.Create<string>();

            var actualAppSettingValue = AppSettings[appSettingName];

            Assert.That(actualAppSettingValue, Is.Null);
        }

        [Test]
        public void ConnectionStrings_IndexerParameterizedWithString_GivenExistsInConfigManagerWithOnlyValue_ReturnsExpected()
        {
            var connectionStringName = Fixture.Create<string>();
            var expected = Given_ConnectionStringWithOnlyValueExistsInConfigManager(connectionStringName, Fixture.Create<string>());

            var actual = ConnectionStrings[connectionStringName];

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void ConnectionStrings_IndexerParameterizedWithString_GivenExistsInConfigManagerWithValueAndProvider_ReturnsExpected()
        {
            var connectionStringName = Fixture.Create<string>();
            var expected = Given_ConnectionStringWithValueAndProviderExistsInConfigManager(connectionStringName, Fixture.Create<string>(), Fixture.Create<string>());

            var actual = ConnectionStrings[connectionStringName];

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void ConnectionStrings_IndexerParameterizedWithString_GivenExistsInEnvironmentWithOnlyValue_ReturnsExpected()
        {
            var expected = Given_ConnectionStringWithOnlyValueExistsInEnvironment();
            var connectionStringName = expected.Name;

            var actual = ConnectionStrings[connectionStringName];

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void ConnectionStrings_IndexerParameterizedWithString_GivenExistsInEnvironmentWithValueAndProvider_ReturnsExpected()
        {
            var expected = Given_ConnectionStringWithValueAndProviderExistsInEnvironment();
            var connectionStringName = expected.Name;

            var actual = ConnectionStrings[connectionStringName];

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void ConnectionStrings_IndexerParameterizedWithString_GivenExistsInEnvironmentWithOnlyValueAndConfigManagerWithOnlyValue_ReturnsMatchFromEnvironment()
        {
            var expected = Given_ConnectionStringWithOnlyValueExistsInEnvironment();
            var connectionStringName = expected.Name;
            Given_ConnectionStringWithOnlyValueExistsInConfigManager(connectionStringName, Fixture.Create<string>());

            var actual = ConnectionStrings[connectionStringName];

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void ConnectionStrings_IndexerParameterizedWithString_GivenExistsInEnvironmentWithValueAndProviderAndExistsInConfigManagerWithAndProvider_ReturnsMatchFromEnvironment()
        {
            var expected = Given_ConnectionStringWithValueAndProviderExistsInEnvironment();
            var connectionStringName = expected.Name;
            Given_ConnectionStringWithValueAndProviderExistsInConfigManager(connectionStringName, Fixture.Create<string>(), Fixture.Create<string>());

            var actual = ConnectionStrings[connectionStringName];

            Assert_ActualConnectionStringEqualsExpected(actual, expected);
        }

        [Test]
        public void ConnectionStrings_IndexerParameterizedWithString_GivenNotExists_ReturnsNull()
        {
            var connectionStringName = Fixture.Create<string>();

            var actualConnectionStringEntry = AppSettings[connectionStringName];

            Assert.That(actualConnectionStringEntry, Is.Null);
        }

        private void Assert_ActualConnectionStringEqualsExpected(ConnectionStringSettings actual, ConnectionStringSettings expected)
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, AssertionConstraintForMatching(new ConstraintExpression(), expected));
        }

        private Constraint AssertionConstraintForMatching(ConstraintExpression leftHas, ConnectionStringSettings expected)
        {
            return
                leftHas.Property(nameof(ConnectionStringSettings.Name)).EqualTo(expected.Name)
                    .And.Property(nameof(ConnectionStringSettings.ConnectionString)).EqualTo(expected.ConnectionString)
                    .And.Property(nameof(ConnectionStringSettings.ProviderName)).EqualTo(expected.ProviderName);
        }

        private ConnectionStringSettings Given_ConnectionStringWithValueAndProviderExistsInEnvironment()
        {
            var connectionString = new ConnectionStringSettings(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>());
            Given_ConnectionStringExistsInEnvironment(connectionString);
            return connectionString;
        }

        private ConnectionStringSettings Given_ConnectionStringWithOnlyValueExistsInEnvironment()
        {
            var connectionString = new ConnectionStringSettings(Fixture.Create<string>(), Fixture.Create<string>());
            Given_ConnectionStringExistsInEnvironment(connectionString);
            return connectionString;
        }

        private void Given_ConnectionStringExistsInEnvironment(ConnectionStringSettings connectionString)
        {
            Given_ConnectionStringValueExistsInEnvironment(connectionString.Name, connectionString.ConnectionString);
            if (!string.IsNullOrWhiteSpace(connectionString.ProviderName))
            {
                Given_ConnectionStringProviderExistsInEnvironment(connectionString.Name, connectionString.ProviderName);
            }
        }

        private void Given_ConnectionStringValueExistsInEnvironment(string name, string value)
        {
            EnvironmentVariables.Add($"PUMP_{name}_ConnectionString", value);
        }

        private void Given_ConnectionStringProviderExistsInEnvironment(string name, string provider)
        {
            EnvironmentVariables.Add($"PUMP_{name}_Provider", provider);
        }

        private ConnectionStringSettings Given_ConnectionStringWithOnlyValueExistsInConfigManager(string name, string value)
        {
            var result = new ConnectionStringSettings(name, value);
            _configurationManagerConnectionStringsContent.Add(result);
            return result;
        }

        private ConnectionStringSettings Given_ConnectionStringWithValueAndProviderExistsInConfigManager(string name, string value, string provider)
        {
            var result = new ConnectionStringSettings(name, value, provider);
            _configurationManagerConnectionStringsContent.Add(result);
            return result;
        }

        private void Given_AppSettingExistsInEnvironment(string name, string value)
        {
            EnvironmentVariables.Add($"PUMP_{name}", value);
        }

        private void Given_AppSettingExistsInConfigManager(string name, string value)
        {
            _configurationManagerAppSettingsContent.Add(name, value);
        }

        [TearDown]
        public void Dispose()
        {
            _lazyConnectionStrings = null;
            _lazyAppSettings = null;

            if (_lazyContainer != null && _lazyContainer.IsValueCreated)
            {
                _lazyContainer.Value.Dispose();
            }

            _lazyContainer = null;
            _mockConfigurationManager = null;
            _mockConfigurationManagerConnectionStrings = null;
            _mockConfigurationManagerAppSettings = null;
        }

        private void SetupMockConfigurationManager()
        {
            var keyComparer = StringComparer.OrdinalIgnoreCase;
            _configurationManagerAppSettingsContent = new NameValueCollection(keyComparer);
            _mockConfigurationManagerAppSettings = Fixture.Mock<IConfigurationManagerAppSettings>();
            _mockConfigurationManagerAppSettings.SetupGet(aps => aps.Keys).Returns(() => _configurationManagerAppSettingsContent.Keys);
            _mockConfigurationManagerAppSettings.SetupGet(aps => aps.AllKeys).Returns(() => _configurationManagerAppSettingsContent.AllKeys);
            _mockConfigurationManagerAppSettings.SetupGet(aps => aps.Count).Returns(() => _configurationManagerAppSettingsContent.Count);
            _mockConfigurationManagerAppSettings.Setup(aps => aps[It.IsAny<string>()]).Returns((string key) => _configurationManagerAppSettingsContent[key]);
            _configurationManagerConnectionStringsContent = new List<ConnectionStringSettings>();
            _mockConfigurationManagerConnectionStrings = Fixture.Mock<IConfigurationManagerConnectionStrings>();
            _mockConfigurationManagerConnectionStrings.Setup(cs => cs[It.IsAny<string>()]).Returns((string key) => _configurationManagerConnectionStringsContent.FirstOrDefault(cs => keyComparer.Equals(cs.Name, key)));
            _mockConfigurationManagerConnectionStrings.As<IEnumerable<ConnectionStringSettings>>().Setup(cs => cs.GetEnumerator()).Returns(() => _configurationManagerConnectionStringsContent.GetEnumerator());
            _mockConfigurationManager = Fixture.Mock<IConfigurationManager>();
            _mockConfigurationManager.SetupGet(cm => cm.AppSettings).Returns(_mockConfigurationManagerAppSettings.Object);
            _mockConfigurationManager.SetupGet(cm => cm.ConnectionStrings).Returns(_mockConfigurationManagerConnectionStrings.Object);
        }

        public class ConfigurationModuleUnderTest : ConfigurationModule
        {
            private readonly Func<IConfigurationManager> _configurationManagerFactory;
            private readonly Func<IDictionary<string, string>> _environmentVariablesFactory;

            public ConfigurationModuleUnderTest(Func<IConfigurationManager> configurationManagerFactory, Func<IDictionary<string, string>> environmentVariablesFactory)
            {
                _configurationManagerFactory = configurationManagerFactory;
                _environmentVariablesFactory = environmentVariablesFactory;
            }

            protected override System.Configuration.Abstractions.IConfigurationManager GetConfigurationManager() => _configurationManagerFactory();
            protected override IDictionary<string, string> GetEnvironmentVariables() => _environmentVariablesFactory();
        }
    }
}
