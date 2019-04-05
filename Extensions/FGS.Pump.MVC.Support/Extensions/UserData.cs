using System;

namespace FGS.Pump.MVC.Support.Extensions
{
    public class UserData
    {
        public Guid UserId { get; set; }

        public UserData()
        {
            UserId = Guid.Empty;
        }
    }
}