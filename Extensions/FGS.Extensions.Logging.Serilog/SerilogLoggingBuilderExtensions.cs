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

#if LOGGING_BUILDER

using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using ILogger = Serilog.ILogger;

namespace FGS.Extensions.Logging.Serilog
{
    /// <summary>
    /// Extends <see cref="ILoggingBuilder"/> with Serilog configuration methods.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/serilog/serilog-extensions-logging/blob/767f884a1dafb033a8232c6c8e61b8f4bf5db6ed/src/Serilog.Extensions.Logging/SerilogLoggingBuilderExtensions.cs. </remarks>
    public static class SerilogLoggingBuilderExtensions
    {
        /// <summary>
        /// Adds Serilog to the logging pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="Microsoft.Extensions.Logging.ILoggingBuilder" /> to add a <see cref="SerilogLoggerProvider"/> to.</param>
        /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="Log"/> will be used.</param>
        /// <param name="dispose">When true, dispose <paramref name="logger"/> when the framework disposes the provider. If the
        /// logger is not specified but <paramref name="dispose"/> is true, the <see cref="Log.CloseAndFlush()"/> method will be
        /// called on the static <see cref="Log"/> class instead.</param>
        /// <returns>The logger factory.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The framework takes ownership of the ILoggerProvider's lifetime management after we hand it over")]
        public static ILoggingBuilder AddSerilog(this ILoggingBuilder builder, ILogger logger = null, bool dispose = false)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            if (dispose)
            {
                builder.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>(services => new SerilogLoggerProvider(logger, true));
            }
            else
            {
                builder.AddProvider(new SerilogLoggerProvider(logger));
            }

            builder.AddFilter<SerilogLoggerProvider>(null, LogLevel.Trace);

            return builder;
        }
    }
}

#endif // LOGGING_BUILDER
