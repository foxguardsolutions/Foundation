using System;

using Autofac;

using Microsoft.Extensions.Options;

namespace FGS.Autofac.Options
{
    using Options = Microsoft.Extensions.Options.Options;

    /// <summary>
    /// Used to configure TOptions instances.
    /// </summary>
    /// <typeparam name="TOptions">The type of options being requested.</typeparam>
    /// <remarks>Taken and modified from: https://github.com/aspnet/Extensions/blob/e311a440d51cddd8f645160d9e27a574c77955a1/src/Options/Options/src/OptionsBuilder.cs. </remarks>
    public class AutofacOptionsBuilder<TOptions>
        where TOptions : class
    {
        /// <summary>
        /// Gets the default name of the TOptions instance.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="global::Autofac.ContainerBuilder"/> for the options being configured.
        /// </summary>
        public ContainerBuilder ContainerBuilder { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> for the options being configured.</param>
        /// <param name="name">The default name of the TOptions instance, if null Options.DefaultName is used.</param>
        public AutofacOptionsBuilder(ContainerBuilder containerBuilder, string name)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));

            ContainerBuilder = containerBuilder;
            Name = name ?? Options.DefaultName;
        }

        /// <summary>
        /// Registers an action used to configure a particular type of options.
        /// Note: These are run before all <seealso cref="PostConfigure(Action{TOptions})"/>.
        /// </summary>
        /// <param name="configureOptions">The action used to configure the options.</param>
        public virtual AutofacOptionsBuilder<TOptions> Configure(Action<TOptions> configureOptions)
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(_ => new ConfigureNamedOptions<TOptions>(Name, configureOptions)).As<IConfigureOptions<TOptions>>().SingleInstance();

            return this;
        }

        public virtual AutofacOptionsBuilder<TOptions> Configure<TDep>(Action<TOptions, TDep> configureOptions)
            where TDep : class
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(ctx => new ConfigureNamedOptions<TOptions, TDep>(Name, ctx.Resolve<TDep>(), configureOptions)).As<IConfigureOptions<TOptions>>().InstancePerDependency();

            return this;
        }

        public virtual AutofacOptionsBuilder<TOptions> Configure<TDep1, TDep2>(Action<TOptions, TDep1, TDep2> configureOptions)
            where TDep1 : class
            where TDep2 : class
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(ctx => new ConfigureNamedOptions<TOptions, TDep1, TDep2>(Name, ctx.Resolve<TDep1>(), ctx.Resolve<TDep2>(), configureOptions)).As<IConfigureOptions<TOptions>>().InstancePerDependency();

            return this;
        }

        public virtual AutofacOptionsBuilder<TOptions> Configure<TDep1, TDep2, TDep3>(Action<TOptions, TDep1, TDep2, TDep3> configureOptions)
            where TDep1 : class
            where TDep2 : class
            where TDep3 : class
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(
                ctx => new ConfigureNamedOptions<TOptions, TDep1, TDep2, TDep3>(
                    Name,
                    ctx.Resolve<TDep1>(),
                    ctx.Resolve<TDep2>(),
                    ctx.Resolve<TDep3>(),
                    configureOptions))
                .As<IConfigureOptions<TOptions>>()
                .InstancePerDependency();

            return this;
        }

        public virtual AutofacOptionsBuilder<TOptions> Configure<TDep1, TDep2, TDep3, TDep4>(Action<TOptions, TDep1, TDep2, TDep3, TDep4> configureOptions)
            where TDep1 : class
            where TDep2 : class
            where TDep3 : class
            where TDep4 : class
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(
                ctx => new ConfigureNamedOptions<TOptions, TDep1, TDep2, TDep3, TDep4>(
                    Name,
                    ctx.Resolve<TDep1>(),
                    ctx.Resolve<TDep2>(),
                    ctx.Resolve<TDep3>(),
                    ctx.Resolve<TDep4>(),
                    configureOptions))
                .As<IConfigureOptions<TOptions>>()
                .InstancePerDependency();

            return this;
        }

        public virtual AutofacOptionsBuilder<TOptions> Configure<TDep1, TDep2, TDep3, TDep4, TDep5>(Action<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> configureOptions)
            where TDep1 : class
            where TDep2 : class
            where TDep3 : class
            where TDep4 : class
            where TDep5 : class
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(
                ctx => new ConfigureNamedOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5>(
                    Name,
                    ctx.Resolve<TDep1>(),
                    ctx.Resolve<TDep2>(),
                    ctx.Resolve<TDep3>(),
                    ctx.Resolve<TDep4>(),
                    ctx.Resolve<TDep5>(),
                    configureOptions))
                .As<IConfigureOptions<TOptions>>()
                .InstancePerDependency();

            return this;
        }

        /// <summary>
        /// Registers an action used to configure a particular type of options.
        /// Note: These are run after all <seealso cref="Configure(Action{TOptions})"/>.
        /// </summary>
        /// <param name="configureOptions">The action used to configure the options.</param>
        public virtual AutofacOptionsBuilder<TOptions> PostConfigure(Action<TOptions> configureOptions)
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(ctx => new PostConfigureOptions<TOptions>(Name, configureOptions)).As<IPostConfigureOptions<TOptions>>().SingleInstance();

            return this;
        }

        public virtual AutofacOptionsBuilder<TOptions> PostConfigure<TDep>(Action<TOptions, TDep> configureOptions)
            where TDep : class
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(ctx => new PostConfigureOptions<TOptions, TDep>(Name, ctx.Resolve<TDep>(), configureOptions)).As<IPostConfigureOptions<TOptions>>().InstancePerDependency();

            return this;
        }

        public virtual AutofacOptionsBuilder<TOptions> PostConfigure<TDep1, TDep2>(Action<TOptions, TDep1, TDep2> configureOptions)
            where TDep1 : class
            where TDep2 : class
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(ctx => new PostConfigureOptions<TOptions, TDep1, TDep2>(Name, ctx.Resolve<TDep1>(), ctx.Resolve<TDep2>(), configureOptions)).As<IPostConfigureOptions<TOptions>>().InstancePerDependency();

            return this;
        }

        public virtual AutofacOptionsBuilder<TOptions> PostConfigure<TDep1, TDep2, TDep3>(Action<TOptions, TDep1, TDep2, TDep3> configureOptions)
            where TDep1 : class
            where TDep2 : class
            where TDep3 : class
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(
                ctx => new PostConfigureOptions<TOptions, TDep1, TDep2, TDep3>(
                    Name,
                    ctx.Resolve<TDep1>(),
                    ctx.Resolve<TDep2>(),
                    ctx.Resolve<TDep3>(),
                    configureOptions))
                .As<IPostConfigureOptions<TOptions>>()
                .InstancePerDependency();

            return this;
        }

        public virtual AutofacOptionsBuilder<TOptions> PostConfigure<TDep1, TDep2, TDep3, TDep4>(Action<TOptions, TDep1, TDep2, TDep3, TDep4> configureOptions)
            where TDep1 : class
            where TDep2 : class
            where TDep3 : class
            where TDep4 : class
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(
                ctx => new PostConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4>(
                    Name,
                    ctx.Resolve<TDep1>(),
                    ctx.Resolve<TDep2>(),
                    ctx.Resolve<TDep3>(),
                    ctx.Resolve<TDep4>(),
                    configureOptions))
                .As<IPostConfigureOptions<TOptions>>()
                .InstancePerDependency();

            return this;
        }

        public virtual AutofacOptionsBuilder<TOptions> PostConfigure<TDep1, TDep2, TDep3, TDep4, TDep5>(Action<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> configureOptions)
            where TDep1 : class
            where TDep2 : class
            where TDep3 : class
            where TDep4 : class
            where TDep5 : class
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            ContainerBuilder.Register(
                ctx => new PostConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5>(
                    Name,
                    ctx.Resolve<TDep1>(),
                    ctx.Resolve<TDep2>(),
                    ctx.Resolve<TDep3>(),
                    ctx.Resolve<TDep4>(),
                    ctx.Resolve<TDep5>(),
                    configureOptions))
                .As<IPostConfigureOptions<TOptions>>()
                .InstancePerDependency();

            return this;
        }
    }
}
