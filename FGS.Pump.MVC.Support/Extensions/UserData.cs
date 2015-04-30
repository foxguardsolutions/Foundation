using System;

namespace MVCSupport.Extensions
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