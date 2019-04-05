using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace FGS.Pump.Extensions
{
    public static class ObjectAnalyzer
    {
        public static bool HasProperty(object obj, string propertyName)
        {
            var type = obj.GetType();
            if (type == typeof(JObject))
                return ((JObject)obj)[propertyName] != null;
            if (type == typeof(ExpandoObject))
                return ((IDictionary<string, object>)obj).ContainsKey(propertyName);
            return type.GetProperties().Any(p => p.Name == propertyName);
        }
    }
}