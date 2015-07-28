using System.ComponentModel.DataAnnotations;

namespace FGS.Pump.Extensions.ComponentModel.DataAnnotations
{
    public class PasswordPolicyAttribute : MinLengthAttribute
    {
        public PasswordPolicyAttribute(int length = 12) : base(length)
        {
            ErrorMessage = string.Format("Password must be at least {0} characters long", length);
        }
    }
}
