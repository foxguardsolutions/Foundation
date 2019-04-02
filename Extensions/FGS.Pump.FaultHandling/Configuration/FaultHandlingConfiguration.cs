using System;

using Microsoft.Extensions.Configuration;

namespace FGS.Pump.FaultHandling.Configuration
{
    internal sealed class FaultHandlingConfiguration : IFaultHandlingConfiguration
    {
        public int MaxRetries { get; }

        public FaultHandlingConfiguration(IConfiguration configuration)
        {
            var maxRetries = configuration["Retry_Max_Attempts"] ?? "3";
            MaxRetries = Convert.ToInt32(maxRetries);
        }
    }
}
