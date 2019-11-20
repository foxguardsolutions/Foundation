using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FGS.Extensions.Hosting.Middleware
{
    /// <summary>
    /// An implementation of <see cref="IHostBuilder"/> that wraps an inner implementation with the added behavior of intercepting calls
    /// to <see cref="IHostBuilder.Build"/> and wrapping the result with decorator before returning it.
    /// </summary>
    internal sealed class DecoratorApplyingHostBuilderDecorator : IHostBuilder
    {
        private readonly IHostBuilder _decorated;
        private readonly Func<IHost, IHost> _decorateBuildResult;

        internal DecoratorApplyingHostBuilderDecorator(IHostBuilder decorated, Func<IHost, IHost> decorateBuildResult)
        {
            _decorated = decorated;
            _decorateBuildResult = decorateBuildResult;
        }

        IHostBuilder IHostBuilder.ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate) =>
            _decorated.ConfigureHostConfiguration(configureDelegate);

        IHostBuilder IHostBuilder.ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate) =>
            _decorated.ConfigureAppConfiguration(configureDelegate);

        IHostBuilder IHostBuilder.ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate) =>
            _decorated.ConfigureServices(configureDelegate);

        IHostBuilder IHostBuilder.UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) =>
            _decorated.UseServiceProviderFactory(factory);

        IHostBuilder IHostBuilder.UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) =>
            _decorated.UseServiceProviderFactory(factory);

        IHostBuilder IHostBuilder.ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate) =>
            _decorated.ConfigureContainer(configureDelegate);

        IHost IHostBuilder.Build() =>
            _decorateBuildResult(_decorated.Build());

        IDictionary<object, object> IHostBuilder.Properties => _decorated.Properties;
    }
}
