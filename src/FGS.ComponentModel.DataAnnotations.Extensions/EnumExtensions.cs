using System;
using System.ComponentModel.DataAnnotations;

namespace FGS.ComponentModel.DataAnnotations.Extensions
{
    /// <summary>
    /// Extends enums with the ability to retrieve value-specific metadata.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the human-readable name of an enum value. If the member is annotated with <see cref="DisplayAttribute"/> then such is
        /// used as the source of the human-readable name. Otherwise, the name of the enum member itself is used as a fallback value.
        /// </summary>
        /// <param name="value">The enum value to retrieve the human-readable name for.</param>
        /// <returns>Returns the name of the given <paramref name="value"/>.</returns>
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            if (field == null)
                return string.Empty;

            var attributes = field.GetCustomAttributes(typeof(DisplayAttribute), true);
            return attributes.Length > 0 ? ((DisplayAttribute)attributes[0]).GetName() : Enum.GetName(value.GetType(), value);
        }

        /// <summary>
        /// Gets the human-readable description of an enum value. If the member is annotated with <see cref="DisplayAttribute"/> (with a <see cref="DisplayAttribute.Description"/>) then such is
        /// used as the source of the human-readable description. Otherwise, the name of the enum member itself is used as a fallback value.
        /// </summary>
        /// <param name="value">The enum value to retrieve the human-readable name for.</param>
        /// <returns>Returns the description of the given <paramref name="value"/>.</returns>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            if (field == null)
                return string.Empty;

            var attributes = field.GetCustomAttributes(typeof(DisplayAttribute), true);
            return attributes.Length > 0 ? ((DisplayAttribute)attributes[0]).GetDescription() : Enum.GetName(value.GetType(), value);
        }
    }
}
