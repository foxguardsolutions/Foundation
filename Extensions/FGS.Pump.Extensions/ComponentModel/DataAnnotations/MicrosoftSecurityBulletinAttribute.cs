using System.ComponentModel.DataAnnotations;

namespace FGS.Pump.Extensions.ComponentModel.DataAnnotations
{
    public class MicrosoftSecurityBulletinAttribute : RegularExpressionAttribute
    {
        public MicrosoftSecurityBulletinAttribute()
            : base(@"MS\d{2}(-)\d{3}$")
        {
        }
    }
}
