using System.ComponentModel.DataAnnotations;

namespace FGS.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Marks a member as requiring formatting that fits a comma-separated list of Mitre CVE IDs.
    /// </summary>
    /// <remarks>For more information, see: https://cve.mitre.org/cve/identifiers/index.html. </remarks>
    public class CveListAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CveListAttribute"/> class.
        /// </summary>
        public CveListAttribute()
            : base(@"^CVE-\d{4}-(0\d{3}|[1-9]\d{3,})(,\s*CVE-\d{4}-(0\d{3}|[1-9]\d{3,}))*$")
        {
        }
    }
}
