using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using FGS.Pump.Configuration.Abstractions;

namespace FGS.Pump.Configuration.Patterns
{
    public class ConnectionStringsAdaptingEnumerable : IEnumerable<ConnectionStringSettings>
    {
        private readonly IConnectionStrings _adapted;

        public ConnectionStringsAdaptingEnumerable(IConnectionStrings adapted)
        {
            _adapted = adapted;
        }

        public IEnumerator<ConnectionStringSettings> GetEnumerator() =>
            _adapted.Select(kvp => kvp.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
