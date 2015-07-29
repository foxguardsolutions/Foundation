using System;

namespace FGS.Pump.MVC.Support
{
    public class InvalidFileNameException : Exception
    {
        public InvalidFileNameException(string message) : base(message)
        {
            
        }
    }
}