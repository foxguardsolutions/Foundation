using System.Security.Principal;

using MVCSupport.Extensions;

namespace MVCSupport.Principals
{
    public class TemplateUser : GenericPrincipal
    {
        public TemplateUser(IIdentity identity, string[] roles) : base(identity, roles)
        {
        }

        public UserData data { get; set; }
    }
}
