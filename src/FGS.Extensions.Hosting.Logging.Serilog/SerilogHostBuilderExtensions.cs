// Copyright 2017 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using ILogger = Serilog.ILogger;

namespace FGS.Extensions.Hosting.Logging.Serilog
{
    /// <summary>
    /// Extends <see cref="IHostBuilder"/> with functionality to configure Serilog.
    /// </summary>
    public static class SerilogHostBuilderExtensions
    {
        /// <summary>
        /// Sets Serilog as the logging provider.
        /// </summary>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="Log"/> will be used.</param>
        /// <param name="dispose">When true, dispose <paramref name="logger"/> when the framework disposes the provider. If the
        /// logger is not specified but <paramref name="dispose"/> is true, the <see cref="Log.CloseAndFlush()"/> method will be
        /// called on the static <see cref="Log"/> class instead.</param>
        /// <returns>The host builder.</returns>
        /// <example>
        /// <code>
        /// public static IHostBuilder CreateHostBuilder(string[] args) =&gt;
        ///     Host.CreateDefaultBuilder(args)
        ///         .UseSerilog()
        ///         .UseStartup&lt;Startup&gt;();
        /// </code>
        /// </example>
        public static IHostBuilder UseSerilog(this IHostBuilder builder, ILogger logger = null, bool dispose = false)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.ConfigureServices(collection =>
                collection.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(logger, dispose)));
            return builder;
        }

        /// <summary>Sets Serilog as the logging provider.</summary>
        /// <remarks>
        /// A <see cref="HostBuilderContext"/> is supplied so that configuration and hosting information can be used.
        /// The logger will be shut down when application services are disposed.
        /// </remarks>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="configureLogger">The delegate for configuring the <see cref="LoggerConfiguration" /> that will be used to construct a <see cref="Logger" />.</param>
        /// <param name="preserveStaticLogger">Indicates whether to preserve the value of <see cref="Log.Logger"/>.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder UseSerilog(this IHostBuilder builder, Action<HostBuilderContext, LoggerConfiguration> configureLogger, bool preserveStaticLogger = false)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configureLogger == null) throw new ArgumentNullException(nameof(configureLogger));
            builder.ConfigureServices((context, collection) =>
            {
                var loggerConfiguration = new LoggerConfiguration();
                configureLogger(context, loggerConfiguration);
                var logger = loggerConfiguration.CreateLogger();
                if (preserveStaticLogger)
                {
                    collection.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(logger, true));
                }
                else
                {
                    // Passing a `null` logger to `SerilogLoggerFactory` results in disposal via
                    // `Log.CloseAndFlush()`, which additionally replaces the static logger with a no-op.
                    Log.Logger = logger;
                    collection.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(null, true));
                }
            });
            return builder;
        }

        /// <summary>
        /// Sets Serilog as the logging provider, using an already-registered <see cref="ILogger"/>.
        /// </summary>
        /// <param name="builder">The host builder to configure.</param>
        /// <returns>The host builder.</returns>
        /// <example>
        /// <code>
        /// public static IHostBuilder CreateHostBuilder(string[] args) =&gt;
        ///     Host.CreateDefaultBuilder(args)
        ///         .UseSerilogExternallyRegistered()
        ///         .UseStartup&lt;Startup&gt;();
        /// </code>
        /// </example>
        public static IHostBuilder UseSerilogExternallyRegistered(this IHostBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            // This has to be singleton in order to support the ASP.NET Core framework attempting to resolve it in singleton scope, otherwise we would get
            // an exception with the message `Cannot consume scoped service from singleton` during application startup.
            builder.ConfigureServices(collection =>
                collection.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(services.GetRequiredService<ILogger>())));

            return builder;
        }
    }
}
