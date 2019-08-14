using System;

using Microsoft.AspNetCore.Mvc;

namespace FGS.AspNetCore.Mvc.ModelBinding
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class Base64UrlEncodedByteArrayAttribute : ModelBinderAttribute
    {
        public Base64UrlEncodedByteArrayAttribute()
        {
            BinderType = typeof(Base64UrlEncodedByteArrayModelBinder);
        }
    }
}
