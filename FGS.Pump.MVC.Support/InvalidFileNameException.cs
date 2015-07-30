using System;
using System.Runtime.Serialization;

namespace FGS.Pump.MVC.Support
{
    [Serializable]
    public class InvalidFileNameException : Exception
    {
        public InvalidFileNameException()
        {
        }

        public InvalidFileNameException(string message) : base(message)
        {
        }

        public InvalidFileNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidFileNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}