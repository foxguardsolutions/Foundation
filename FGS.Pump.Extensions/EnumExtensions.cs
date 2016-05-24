using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FGS.Pump.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            if (field == null)
                return string.Empty;

            var attributes = field.GetCustomAttributes(typeof(DisplayAttribute), true);
            return attributes.Length > 0 ? ((DisplayAttribute)attributes[0]).GetName() : Enum.GetName(value.GetType(), value);
        }
    }
}