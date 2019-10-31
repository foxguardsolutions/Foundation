using System.ComponentModel.DataAnnotations;

namespace FGS.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Marks a member as requiring formatting that fits the legacy Microsoft security bulletin id number (e.g. "MS16-XXX").
    /// </summary>
    /// <remarks>For more information, see: https://www.microsoft.com/en-us/msrc/faqs-security-update-guide. </remarks>
    public class MicrosoftSecurityBulletinAttribute : RegularExpressionAttribute
    {
        public MicrosoftSecurityBulletinAttribute()
            : base(@"MS\d{2}(-)\d{3}$")
        {
        }
    }
}
