using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using Autofac;

using FGS.Pump.Configuration.Abstractions;
using FGS.Pump.Configuration.Environment;
using FGS.Pump.Configuration.Patterns;
using FGS.Pump.Configuration.Patterns.Specialized;
using FGS.Pump.Extensions.DI;

namespace FGS.Pump.Configuration
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            const string AppSettingNameComparerRegistrationName = "AppSettingNameComparer";
            const string ConnectionStringNameComparerRegistrationName = "ConnectionStringNameComparer";
            const string EnvironmentVariablesRegistrationName = "EnvironmentVariables";

            RegisterSharedConfigurationUtilities(builder, AppSettingNameComparerRegistrationName, ConnectionStringNameComparerRegistrationName, EnvironmentVariablesRegistrationName);
            RegisterAppSettings(builder, AppSettingNameComparerRegistrationName, EnvironmentVariablesRegistrationName);
            RegisterConnectionStrings(builder, ConnectionStringNameComparerRegistrationName, EnvironmentVariablesRegistrationName);
        }

        private void RegisterSharedConfigurationUtilities(ContainerBuilder builder, string appSettingNameComparerRegistrationName, string connectionStringNameComparerRegistrationName, string environmentVariablesRegistrationName)
        {
            builder.Register(ctx => StringComparer.OrdinalIgnoreCase)
                .Named<IEqualityComparer<string>>(appSettingNameComparerRegistrationName)
                .Named<IEqualityComparer<string>>(connectionStringNameComparerRegistrationName)
                .SingleInstance();

            builder
                .Register(ctx => GetConfigurationManager())
                .As<System.Configuration.Abstractions.IConfigurationManager>()
                .SingleInstance();

            builder
                .Register(ctx => GetEnvironmentVariables())
                .Named<IDictionary<string, string>>(environmentVariablesRegistrationName)
                .SingleInstance();
        }

        private void RegisterAppSettings(ContainerBuilder builder, string appSettingNameComparerRegistrationName, string environmentVariablesRegistrationName)
        {
            IEqualityComparer<string> ResolveNameComparer(IComponentContext ctx) => ctx.ResolveNamed<IEqualityComparer<string>>(appSettingNameComparerRegistrationName);

            const string ImplementationFromConfigurationManagerRegistrationName = "AppSettingsFromConfigurationManager";
            builder.RegisterType<ConfigurationManagerAdaptingAppSettings>()
                .Named<IAppSettings>(ImplementationFromConfigurationManagerRegistrationName)
                .WithParameterTypedFromAndNamed("appSettingNameComparer", ResolveNameComparer)
                .SingleInstance();

            builder.RegisterType<EnvironmentKeyAppSettingsKeyAdaptationStrategy>().As<IAppSettingsKeyAdaptationStrategy>().SingleInstance();

            const string EnvironmentBasedEnumerableRegistrationName = "AppSettingsEnvironmentEnumerable";
            builder.RegisterType<StringDictionaryAdaptingAppSettingEnumerable>()
                .Named<IEnumerable<KeyValuePair<string, string>>>(EnvironmentBasedEnumerableRegistrationName)
                .WithParameterTypedFromAndNamed("adapted", ctx => ctx.ResolveNamed<IDictionary<string, string>>(environmentVariablesRegistrationName))
                .SingleInstance();

            const string ImplementationFromEnvironmentEnumerableRegistrationName = "AppSettingsFromEnvironmentEnumerable";
            builder.RegisterType<EnumerableAdaptingAppSettings>()
                .Named<IAppSettings>(ImplementationFromEnvironmentEnumerableRegistrationName)
                .WithParameterTypedFromAndNamed("adapted", ctx => ctx.ResolveNamed<IEnumerable<KeyValuePair<string, string>>>(EnvironmentBasedEnumerableRegistrationName))
                .WithParameterTypedFromAndNamed("appSettingNameComparer", ResolveNameComparer)
                .SingleInstance();

            const string CompositeImplementationRegistrationName = "AppSettingsComposite";
            IEnumerable<IAppSettings> ResolveInnerImplementationsInPrecendenceOrder(IComponentContext ctx) =>
                ResolveMultipleNamed<IAppSettings>(ctx, ImplementationFromEnvironmentEnumerableRegistrationName, ImplementationFromConfigurationManagerRegistrationName);
            builder.RegisterType<CompositeAppSettings>()
                .Named<IAppSettings>(CompositeImplementationRegistrationName)
                .WithParameterTypedFromAndNamed("adapted", ResolveInnerImplementationsInPrecendenceOrder)
                .WithParameterTypedFromAndNamed("appSettingNameComparer", ResolveNameComparer)
                .SingleInstance();

            builder.RegisterType<MemoizingAppSettingsDecorator>()
                .As<IAppSettings>()
                .WithParameterTypedFromAndNamed("decorated", ctx => ctx.ResolveNamed<IAppSettings>(CompositeImplementationRegistrationName))
                .WithParameterTypedFromAndNamed("appSettingNameComparer", ResolveNameComparer)
                .SingleInstance();
        }

        private void RegisterConnectionStrings(ContainerBuilder builder, string connectionStringNameComparerRegistrationName, string environmentVariablesRegistrationName)
        {
            IEqualityComparer<string> ResolveNameComparer(IComponentContext ctx) => ctx.ResolveNamed<IEqualityComparer<string>>(connectionStringNameComparerRegistrationName);

            const string ImplementationFromConfigurationManagerAsEnumerableRegistrationName = "ConnectionStringsFromConfigurationManagerAsEnumerable";
            void RegisterImplementationFromConfigurationManagerAsEnumerable()
            {
                const string ImplementationFromConfigurationManagerRegistrationName = "ConnectionStringsFromConfigurationManager";
                builder.RegisterType<ConfigurationManagerAdaptingConnectionStrings>()
                    .Named<IConnectionStrings>(ImplementationFromConfigurationManagerRegistrationName)
                    .WithParameterTypedFromAndNamed("connectionStringNameComparer", ResolveNameComparer)
                    .SingleInstance();

                builder.RegisterType<ConnectionStringsAdaptingEnumerable>()
                    .Named<IEnumerable<ConnectionStringSettings>>(ImplementationFromConfigurationManagerAsEnumerableRegistrationName)
                    .WithParameterTypedFromAndNamed("adapted", ctx => ctx.ResolveNamed<IConnectionStrings>(ImplementationFromConfigurationManagerRegistrationName))
                    .SingleInstance();
            }

            const string ImplementationFromEnvironmentAsEnumerableRegistrationName = "ConnectionStringsFromEnvironmentAsEnumerable";
            void RegisterImplementationFromEnvironmentAsEnumerable()
            {
                builder.RegisterType<EnvironmentKeySplitConnectionStringAdaptationStrategy>().As<ISplitConnectionStringAdaptationStrategy>().SingleInstance();

                builder.RegisterType<SplitConnectionStringDictionaryAdaptingConnectionStringEnumerable>()
                    .Named<IEnumerable<ConnectionStringSettings>>(ImplementationFromEnvironmentAsEnumerableRegistrationName)
                    .WithParameterTypedFromAndNamed("adapted", ctx => ctx.ResolveNamed<IDictionary<string, string>>(environmentVariablesRegistrationName))
                    .WithParameterTypedFromAndNamed("connectionStringNameComparer", ResolveNameComparer)
                    .SingleInstance();
            }

            const string ImplementationMuxingImplementationAsEnumerableRegistrationName = "ConnectionStringsMuxerAsEnumerable";
            void RegisterImplementationMuxingImplementationAsEnumerable()
            {
                const string MuxerSourcesRegistrationName = "ConnectionStringsMuxerSources";
                builder.Register(
                        ctx => ResolveMultipleNamed<IEnumerable<ConnectionStringSettings>>(
                                ctx,
                                ImplementationFromEnvironmentAsEnumerableRegistrationName,
                                ImplementationFromConfigurationManagerAsEnumerableRegistrationName))
                            .Named<IEnumerable<IEnumerable<ConnectionStringSettings>>>(MuxerSourcesRegistrationName)
                    .SingleInstance();

                builder.RegisterType<EnumerableConnectionStringSettingsMultiplexReducer>()
                    .Named<IEnumerable<ConnectionStringSettings>>(ImplementationMuxingImplementationAsEnumerableRegistrationName)
                    .WithParameterTypedFromAndNamed("adapted", ctx => ctx.ResolveNamed<IEnumerable<IEnumerable<ConnectionStringSettings>>>(MuxerSourcesRegistrationName))
                    .WithParameterTypedFromAndNamed("connectionStringNameComparer", ResolveNameComparer)
                    .SingleInstance();
            }

            const string ImplementationFromOverridesAsEnumerableRegistrationName = "ConnectionStringsFromOverridesAsEnumerable";
            void RegisterImplementationFromOverridesAsEnumerable()
            {
                builder.RegisterType<AppSettingsOverriddenSqlServerConnectionStringsAdaptationStrategy>().As<IAppSettingsOverriddenSqlServerConnectionStringsAdaptationStrategy>().SingleInstance();

                const string ConnectionStringPartNameComparerRegistrationName = "ConnectionStringPartNameComparer";
                const string ConnectionStringProviderComparerRegistrationName = "ConnectionStringProviderComparer";
                builder.Register(ctx => StringComparer.OrdinalIgnoreCase)
                    .Named<IEqualityComparer<string>>(ConnectionStringPartNameComparerRegistrationName)
                    .Named<IEqualityComparer<string>>(ConnectionStringProviderComparerRegistrationName)
                    .SingleInstance();

                builder.RegisterType<SqlServerConnectionStringBuilderApplicator>().As<ISqlServerConnectionStringBuilderApplicator>().SingleInstance();

                builder.RegisterType<AppSettingsOverridenSqlServerConnectionStringEnumerable>()
                    .Named<IEnumerable<ConnectionStringSettings>>(ImplementationFromOverridesAsEnumerableRegistrationName)
                    .WithParameterTypedFromAndNamed("adapted", ctx => ctx.ResolveNamed<IEnumerable<ConnectionStringSettings>>(ImplementationMuxingImplementationAsEnumerableRegistrationName))
                    .WithParameterTypedFromAndNamed("connectionStringNameComparer", ResolveNameComparer)
                    .WithParameterTypedFromAndNamed("connectionStringPartNameComparer", ctx => ctx.ResolveNamed<IEqualityComparer<string>>(ConnectionStringPartNameComparerRegistrationName))
                    .WithParameterTypedFromAndNamed("connectionStringProviderComparer", ctx => ctx.ResolveNamed<IEqualityComparer<string>>(ConnectionStringProviderComparerRegistrationName))
                    .SingleInstance();
            }

            const string ImplementationFromEnumerableRegistrationName = "ConnectionStringsFromEnumerable";
            void RegisterImplementationFromEnumerable()
            {
                builder.RegisterType<EnumerableAdaptingConnectionStrings>()
                    .Named<IConnectionStrings>(ImplementationFromEnumerableRegistrationName)
                    .WithParameterTypedFromAndNamed("adapted", ctx => ctx.ResolveNamed<IEnumerable<ConnectionStringSettings>>(ImplementationFromOverridesAsEnumerableRegistrationName))
                    .WithParameterTypedFromAndNamed("connectionStringNameComparer", ResolveNameComparer)
                    .SingleInstance();
            }

            RegisterImplementationFromConfigurationManagerAsEnumerable();
            RegisterImplementationFromEnvironmentAsEnumerable();
            RegisterImplementationMuxingImplementationAsEnumerable();
            RegisterImplementationFromOverridesAsEnumerable();
            RegisterImplementationFromEnumerable();

            builder.RegisterType<MemoizingConnectionStringsDecorator>()
                .As<IConnectionStrings>()
                .WithParameterTypedFromAndNamed("decorated", ctx => ctx.ResolveNamed<IConnectionStrings>(ImplementationFromEnumerableRegistrationName))
                .WithParameterTypedFromAndNamed("connectionStringNameComparer", ResolveNameComparer)
                .SingleInstance();
        }

        protected virtual System.Configuration.Abstractions.IConfigurationManager GetConfigurationManager() => new System.Configuration.Abstractions.ConfigurationManager();

        protected virtual IDictionary<string, string> GetEnvironmentVariables()
        {
            var originalDictionary = System.Environment.GetEnvironmentVariables();
            return
                originalDictionary
                .Keys
                .Cast<object>()
                .Select(k => new KeyValuePair<string, string>((string)k, (string)originalDictionary[k]))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private static IEnumerable<T> ResolveMultipleNamed<T>(IComponentContext context, params string[] names) => names.Select(context.ResolveNamed<T>).ToArray();
    }
}
