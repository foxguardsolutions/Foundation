using System.Security.Principal;

using FGS.Pump.MVC.Support.Extensions;

namespace FGS.Pump.MVC.Support.Principals
{
    public class TemplateUser : GenericPrincipal
    {
        public TemplateUser(IIdentity identity, string[] roles) : base(identity, roles)
        {
        }

        public UserData data { get; set; }
    }
}
