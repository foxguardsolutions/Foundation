using System;

namespace FGS.Pump.MVC.Support.Extensions
{
    public class UserData  
    {
        public Guid userID { get; set; }

        public UserData()
        {
            userID = Guid.Empty;
        }
    }
}