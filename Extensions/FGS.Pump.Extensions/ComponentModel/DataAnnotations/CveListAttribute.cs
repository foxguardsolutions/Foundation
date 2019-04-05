using System.ComponentModel.DataAnnotations;

namespace FGS.Pump.Extensions.ComponentModel.DataAnnotations
{
    public class CveListAttribute : RegularExpressionAttribute
    {
        public CveListAttribute()
            : base(@"^CVE-\d{4}-(0\d{3}|[1-9]\d{3,})(,\s*CVE-\d{4}-(0\d{3}|[1-9]\d{3,}))*$")
        {
        }
    }
}
