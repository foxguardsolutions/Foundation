using System.Collections.Generic;
using System.Configuration;

namespace FGS.Pump.Configuration.Abstractions
{
    public interface IConnectionStrings : IReadOnlyDictionary<string, ConnectionStringSettings>
    {
    }
}
