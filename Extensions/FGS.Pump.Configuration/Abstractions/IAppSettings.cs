using System.Collections.Generic;

namespace FGS.Pump.Configuration.Abstractions
{
    public interface IAppSettings : IReadOnlyDictionary<string, string>
    {
    }
}
