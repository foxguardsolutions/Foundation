using System.ComponentModel.DataAnnotations;

namespace FGS.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Marks a member as requiring formatting that fits a Mitre CVE ID.
    /// </summary>
    /// <remarks>For more information, see: https://cve.mitre.org/cve/identifiers/index.html. </remarks>
    public class CveAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CveAttribute"/> class.
        /// </summary>
        public CveAttribute()
            : base(@"^CVE-\d{4}-(0\d{3}|[1-9]\d{3,})$")
        {
        }
    }
}
