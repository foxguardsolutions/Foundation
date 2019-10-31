using System;

using Autofac;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FGS.Autofac.Options
{
    using Options = Microsoft.Extensions.Options.Options;

    /// <summary>
    /// Extension methods for adding configuration-related Options services to the Autofac-based DI container.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/aspnet/Extensions/blob/e311a440d51cddd8f645160d9e27a574c77955a1/src/Options/ConfigurationExtensions/src/OptionsConfigurationServiceCollectionExtensions.cs. </remarks>
    public static class OptionsConfigurationContainerBuilderExtensions
    {
        /// <summary>
        /// Registers a configuration instance which <typeparamref name="TOptions" /> will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of Options being configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder containerBuilder, IConfiguration config)
            where TOptions : class
            => containerBuilder.Configure<TOptions>(Options.DefaultName, config);

        /// <summary>
        /// Registers a configuration instance which <typeparamref name="TOptions" /> will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of Options being configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="name">The name of the Options instance.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder containerBuilder, string name, IConfiguration config)
            where TOptions : class
            => containerBuilder.Configure<TOptions>(name, config, _ => { });

        /// <summary>
        /// Registers a configuration instance which <typeparamref name="TOptions" /> will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of Options being configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder containerBuilder, IConfiguration config, Action<BinderOptions> configureBinder)
            where TOptions : class
            => containerBuilder.Configure<TOptions>(Options.DefaultName, config, configureBinder);

        /// <summary>
        /// Registers a configuration instance which <typeparamref name="TOptions" /> will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of Options being configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="name">The name of the Options instance.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder containerBuilder, string name, IConfiguration config, Action<BinderOptions> configureBinder)
            where TOptions : class
            => containerBuilder.Configure<TOptions>(name, _ => config, configureBinder);

        /// <summary>
        /// Registers a configuration instance which <typeparamref name="TOptions" /> will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of Options being configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="resolveConfig">Resolves the configuration being bound.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder containerBuilder, Func<IComponentContext, IConfiguration> resolveConfig)
            where TOptions : class
            => containerBuilder.Configure<TOptions>(Options.DefaultName, resolveConfig);

        /// <summary>
        /// Registers a configuration instance which <typeparamref name="TOptions" /> will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of Options being configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="name">The name of the Options instance.</param>
        /// <param name="resolveConfig">Resolves the configuration being bound.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder containerBuilder, string name, Func<IComponentContext, IConfiguration> resolveConfig)
            where TOptions : class
            => containerBuilder.Configure<TOptions>(name, resolveConfig, _ => { });

        /// <summary>
        /// Registers a configuration instance which <typeparamref name="TOptions" /> will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of Options being configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="resolveConfig">Resolves the configuration being bound.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder containerBuilder, Func<IComponentContext, IConfiguration> resolveConfig, Action<BinderOptions> configureBinder)
            where TOptions : class
            => containerBuilder.Configure<TOptions>(Options.DefaultName, resolveConfig, configureBinder);

        /// <summary>
        /// Registers a configuration instance which <typeparamref name="TOptions" /> will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of Options being configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="name">The name of the Options instance.</param>
        /// <param name="resolveConfig">Resolves the configuration being bound.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder containerBuilder, string name, Func<IComponentContext, IConfiguration> resolveConfig, Action<BinderOptions> configureBinder)
            where TOptions : class
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            if (resolveConfig == null) throw new ArgumentNullException(nameof(resolveConfig));

            containerBuilder.AddOptions();
            containerBuilder.Register(ctx => new ConfigurationChangeTokenSource<TOptions>(name, resolveConfig(ctx))).As<IOptionsChangeTokenSource<TOptions>>().SingleInstance();
            containerBuilder.Register(ctx => new NamedConfigureFromConfigurationOptions<TOptions>(name, resolveConfig(ctx), configureBinder)).As<IConfigureOptions<TOptions>>().SingleInstance();

            return containerBuilder;
        }
    }
}
