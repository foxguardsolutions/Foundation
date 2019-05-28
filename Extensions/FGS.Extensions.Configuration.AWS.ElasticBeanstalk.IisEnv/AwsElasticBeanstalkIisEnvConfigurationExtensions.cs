using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace FGS.Extensions.Configuration.AWS.ElasticBeanstalk.IisEnv
{
    /// <summary>
    /// Extension methods for adding <see cref="AwsElasticBeanstalkIisEnvConfigurationProvider"/>.
    /// </summary>
    /// <remarks>Based on: https://github.com/aspnet/Extensions/blob/67394a5c9f92832d31d871c124763bc6231c9009/src/Configuration/Config.Json/src/JsonConfigurationExtensions.cs </remarks>
    public static class AwsElasticBeanstalkIisEnvConfigurationExtensions
    {
        private const string JsonFilePath = @"C:\Program Files\Amazon\ElasticBeanstalk\config\containerconfiguration";

        /// <summary>
        /// Adds the AWS Elastic Beanstalk IIS Env configuration provider to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAwsElasticBeanstalkIisEnvConfiguration(this IConfigurationBuilder builder)
        {
            return AddAwsElasticBeanstalkIisEnvConfiguration(builder, provider: null, optional: false, reloadOnChange: false);
        }

        /// <summary>
        /// Adds the AWS Elastic Beanstalk IIS Env configuration provider to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="optional">Whether the underlying "container configuration" file is optional.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAwsElasticBeanstalkIisEnvConfiguration(this IConfigurationBuilder builder, bool optional)
        {
            return AddAwsElasticBeanstalkIisEnvConfiguration(builder, provider: null, optional: optional, reloadOnChange: false);
        }

        /// <summary>
        /// Adds the AWS Elastic Beanstalk IIS Env configuration provider to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="optional">Whether the underlying "container configuration" file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the underlying "container configuration" file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAwsElasticBeanstalkIisEnvConfiguration(this IConfigurationBuilder builder, bool optional, bool reloadOnChange)
        {
            return AddAwsElasticBeanstalkIisEnvConfiguration(builder, provider: null, optional: optional, reloadOnChange: reloadOnChange);
        }

        /// <summary>
        /// Adds an AWS Elastic Beanstalk IIS Env configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the underlying "container configuration" file.</param>
        /// <param name="optional">Whether the underlying "container configuration" file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the underlying "container configuration" file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAwsElasticBeanstalkIisEnvConfiguration(this IConfigurationBuilder builder, IFileProvider provider, bool optional, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddAwsElasticBeanstalkIisEnvConfiguration(s =>
            {
                s.FileProvider = provider;
                s.Path = JsonFilePath;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.ResolveFileProvider();
            });
        }

        /// <summary>
        /// Adds an AWS Elastic Beanstalk IIS Env configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAwsElasticBeanstalkIisEnvConfiguration(this IConfigurationBuilder builder, Action<AwsElasticBeanstalkContainerConfigurationConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}
