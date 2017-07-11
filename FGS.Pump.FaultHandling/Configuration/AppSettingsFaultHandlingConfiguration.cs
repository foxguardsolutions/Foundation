using System;
using System.Configuration.Abstractions;

namespace FGS.Pump.FaultHandling.Configuration
{
    internal sealed class AppSettingsFaultHandlingConfiguration : IFaultHandlingConfiguration
    {
        public int MaxRetries { get; }

        public AppSettingsFaultHandlingConfiguration(IAppSettings appSettings)
        {
            var maxRetries = appSettings["FaultHandling_Retry_Max_Attempts"];
            MaxRetries = Convert.ToInt32(maxRetries);
        }
    }
}