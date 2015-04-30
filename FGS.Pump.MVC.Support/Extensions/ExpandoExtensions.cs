using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace FGS.Pump.MVC.Support.Extensions
{
    public static class ExpandoExtensions
    {
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
            {
                var obj = propertyDescriptor.GetValue(anonymousObject);
                expando.Add(propertyDescriptor.Name, obj);
            }

            return (ExpandoObject)expando;
        }

        public static bool HasAttr(ExpandoObject expando, string key)
        {
            return ((IDictionary<string, Object>)expando).ContainsKey(key);
        }
    }
}
