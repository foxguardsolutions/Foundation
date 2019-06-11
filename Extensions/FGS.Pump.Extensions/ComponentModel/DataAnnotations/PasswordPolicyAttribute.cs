﻿using System.ComponentModel.DataAnnotations;

namespace FGS.Pump.Extensions.ComponentModel.DataAnnotations
{
    public class PasswordPolicyAttribute : MinLengthAttribute
    {
        public PasswordPolicyAttribute(int length = 12)
            : base(length)
        {
            ErrorMessage = $"Password must be at least {length} characters long";
        }
    }
}