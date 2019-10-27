using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Builder;

using Microsoft.Extensions.Options;

namespace FGS.Autofac.Options
{
    using Options = Microsoft.Extensions.Options.Options;

    /// <summary>
    /// Extension methods for options services to the Autofac-based DI container.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/aspnet/Extensions/blob/e311a440d51cddd8f645160d9e27a574c77955a1/src/Options/Options/src/OptionsServiceCollectionExtensions.cs. </remarks>
    public static class OptionsContainerBuilderExtensions
    {
        /// <summary>
        /// Adds services required for using options.
        /// </summary>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder AddOptions(this ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));

            IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> RegisterGenericIfNotRegistered(Type implementation, Type service)
                => containerBuilder.RegisterGeneric(implementation).AsSelf().As(service).IfNotRegistered(service);

            RegisterGenericIfNotRegistered(typeof(OptionsManager<>), typeof(IOptions<>)).SingleInstance();
            RegisterGenericIfNotRegistered(typeof(OptionsManager<>), typeof(IOptionsSnapshot<>)).InstancePerLifetimeScope();
            RegisterGenericIfNotRegistered(typeof(OptionsMonitor<>), typeof(IOptionsMonitor<>)).SingleInstance();
            RegisterGenericIfNotRegistered(typeof(OptionsFactory<>), typeof(IOptionsFactory<>)).InstancePerDependency();
            RegisterGenericIfNotRegistered(typeof(OptionsCache<>), typeof(IOptionsMonitorCache<>)).InstancePerDependency();

            return containerBuilder;
        }

        /// <summary>
        /// Registers an action used to configure a particular type of options.
        /// Note: These are run before all <seealso cref="PostConfigure{TOptions}(ContainerBuilder, Action{TOptions})"/>.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="configureOptions">The action used to configure the options.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder containerBuilder, Action<TOptions> configureOptions)
            where TOptions : class
            => containerBuilder.Configure(Options.DefaultName, configureOptions);

        /// <summary>
        /// Registers an action used to configure a particular type of options.
        /// Note: These are run before all <seealso cref="PostConfigure{TOptions}(ContainerBuilder, Action{TOptions})"/>.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="configureOptions">The action used to configure the options.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder containerBuilder, string name, Action<TOptions> configureOptions)
            where TOptions : class
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            containerBuilder.AddOptions();
            containerBuilder.Register(_ => new ConfigureNamedOptions<TOptions>(name, configureOptions)).As<IConfigureOptions<TOptions>>().SingleInstance();

            return containerBuilder;
        }

        /// <summary>
        /// Registers an action used to configure all instances of a particular type of options.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="configureOptions">The action used to configure the options.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder ConfigureAll<TOptions>(this ContainerBuilder containerBuilder, Action<TOptions> configureOptions)
            where TOptions : class
            => containerBuilder.Configure(name: null, configureOptions: configureOptions);

        /// <summary>
        /// Registers an action used to initialize a particular type of options.
        /// Note: These are run after all <seealso cref="Configure{TOptions}(ContainerBuilder, Action{TOptions})"/>.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="configureOptions">The action used to configure the options.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder PostConfigure<TOptions>(this ContainerBuilder containerBuilder, Action<TOptions> configureOptions)
            where TOptions : class
            => containerBuilder.PostConfigure(Options.DefaultName, configureOptions);

        /// <summary>
        /// Registers an action used to configure a particular type of options.
        /// Note: These are run after all <seealso cref="Configure{TOptions}(ContainerBuilder, Action{TOptions})"/>.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configure.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="configureOptions">The action used to configure the options.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder PostConfigure<TOptions>(this ContainerBuilder containerBuilder, string name, Action<TOptions> configureOptions)
            where TOptions : class
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            containerBuilder.AddOptions();
            containerBuilder.Register(_ => new PostConfigureOptions<TOptions>(name, configureOptions)).As<IPostConfigureOptions<TOptions>>().SingleInstance();

            return containerBuilder;
        }

        /// <summary>
        /// Registers an action used to post configure all instances of a particular type of options.
        /// Note: These are run after all <seealso cref="Configure{TOptions}(ContainerBuilder, Action{TOptions})"/>.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="configureOptions">The action used to configure the options.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder PostConfigureAll<TOptions>(this ContainerBuilder containerBuilder, Action<TOptions> configureOptions)
            where TOptions : class
            => containerBuilder.PostConfigure(name: null, configureOptions: configureOptions);

        /// <summary>
        /// Registers a type that will have all of its I[Post]ConfigureOptions registered.
        /// </summary>
        /// <typeparam name="TConfigureOptions">The type that will configure options.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder ConfigureOptions<TConfigureOptions>(this ContainerBuilder containerBuilder)
            where TConfigureOptions : class
            => containerBuilder.ConfigureOptions(typeof(TConfigureOptions));

        private static bool IsAction(Type type)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Action<>);

        private static IEnumerable<Type> FindIConfigureOptions(Type type)
        {
            var serviceTypes = type.GetTypeInfo().ImplementedInterfaces
                .Where(t => t.GetTypeInfo().IsGenericType &&
                (t.GetGenericTypeDefinition() == typeof(IConfigureOptions<>)
                || t.GetGenericTypeDefinition() == typeof(IPostConfigureOptions<>)));

            if (!serviceTypes.Any())
            {
                throw new InvalidOperationException(
                    IsAction(type)
                    ? "No IConfigureOptions<> or IPostConfigureOptions<> implementations were found, did you mean to call Configure<> or PostConfigure<>?"
                    : "No IConfigureOptions<> or IPostConfigureOptions<> implementations were found.");
            }

            return serviceTypes;
        }

        /// <summary>
        /// Registers a type that will have all of its I[Post]ConfigureOptions registered.
        /// </summary>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="configureType">The type that will configure options.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder ConfigureOptions(this ContainerBuilder containerBuilder, Type configureType)
        {
            containerBuilder.AddOptions();

            var serviceTypes = FindIConfigureOptions(configureType);
            foreach (var serviceType in serviceTypes)
            {
                containerBuilder.RegisterType(configureType).As(serviceType).InstancePerDependency();
            }

            return containerBuilder;
        }

        /// <summary>
        /// Registers an object that will have all of its I[Post]ConfigureOptions registered.
        /// </summary>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="configureInstance">The instance that will configure options.</param>
        /// <returns>The <see cref="ContainerBuilder"/> so that additional calls can be chained.</returns>
        public static ContainerBuilder ConfigureOptions(this ContainerBuilder containerBuilder, object configureInstance)
        {
            containerBuilder.AddOptions();

            var serviceTypes = FindIConfigureOptions(configureInstance.GetType());
            foreach (var serviceType in serviceTypes)
            {
                containerBuilder.RegisterInstance(configureInstance).As(serviceType).SingleInstance();
            }

            return containerBuilder;
        }

        /// <summary>
        /// Gets an options builder that forwards Configure calls for the same <typeparamref name="TOptions"/> to the underlying service collection.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <returns>The <see cref="OptionsBuilder{TOptions}"/> so that configure calls can be chained in it.</returns>
        public static AutofacOptionsBuilder<TOptions> AddOptions<TOptions>(this ContainerBuilder containerBuilder)
            where TOptions : class
            => containerBuilder.AddOptions<TOptions>(Options.DefaultName);

        /// <summary>
        /// Gets an options builder that forwards Configure calls for the same named <typeparamref name="TOptions"/> to the underlying service collection.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the services to.</param>
        /// <param name="name">The name of the options instance.</param>
        /// <returns>The <see cref="OptionsBuilder{TOptions}"/> so that configure calls can be chained in it.</returns>
        public static AutofacOptionsBuilder<TOptions> AddOptions<TOptions>(this ContainerBuilder containerBuilder, string name)
            where TOptions : class
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));

            containerBuilder.AddOptions();

            return new AutofacOptionsBuilder<TOptions>(containerBuilder, name);
        }
    }
}
