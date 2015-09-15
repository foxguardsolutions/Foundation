using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FGS.Pump.Extensions.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Safely validates a phone number.
    /// </summary>
    /// <remarks>
    /// Enforces a timeout when matching a phone number to the regex to prevent a denial of service attack.
    /// See https://www.owasp.org/index.php/Regular_expression_Denial_of_Service_-_ReDoS for more info.
    /// </remarks>
    public sealed class SafePhoneNumberAttribute : DataTypeAttribute
    {
        private static readonly Regex Regex = new Regex("^(\\+\\s?)?((?<!\\+.*)\\(\\+?\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+)([\\s\\-\\.]?(\\(\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+))*(\\s?(x|ext\\.?)\\s?\\d+)?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

        public SafePhoneNumberAttribute()
            : base(DataType.PhoneNumber)
        {
            this.ErrorMessage = "This phone number is not valid";
        }

        /// <summary>
        /// Determines whether the specified value matches the pattern of a phone number.
        /// </summary>
        /// <returns>
        /// true if the specified value is valid or null; otherwise, false.
        /// </returns>
        /// <param name="value">The value to validate.</param>
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            string input = value as string;
            if (input != null)
                return Regex.Match(input).Length > 0;
            return false;
        }
    }
}