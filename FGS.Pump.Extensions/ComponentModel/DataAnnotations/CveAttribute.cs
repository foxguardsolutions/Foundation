using System.ComponentModel.DataAnnotations;

namespace FGS.Pump.Extensions.ComponentModel.DataAnnotations
{
    public class CveAttribute : RegularExpressionAttribute
    {
        public CveAttribute()
            : base(@"^CVE-\d{4}-(0\d{3}|[1-9]\d{3,})$")
        {
        }
    }
}
