using System.ComponentModel.DataAnnotations;

namespace FGS.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Marks a member as having a minimum-length as per a password complexity policy.
    /// </summary>
    public class PasswordPolicyAttribute : MinLengthAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordPolicyAttribute"/> class.
        /// </summary>
        /// <param name="length">The minimum required length of the annotated member. Defaults to <value>12</value>.</param>
        public PasswordPolicyAttribute(int length = 12)
            : base(length)
        {
            ErrorMessage = $"Password must be at least {length} characters long";
        }
    }
}
