using System.Security.Principal;

using FGS.Pump.MVC.Support.Extensions;

namespace FGS.Pump.MVC.Support.Principals
{
    public class User : GenericPrincipal
    {
        public User(IIdentity identity, string[] roles)
            : base(identity, roles)
        {
        }

        public UserData Data { get; set; }
    }
}
