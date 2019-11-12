using System;

using Microsoft.AspNetCore.Mvc;

namespace FGS.AspNetCore.Mvc.ModelBinding
{
    /// <summary>
    /// Indicates that the given model member should be bound with <see cref="Base64UrlEncodedByteArrayModelBinder"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class Base64UrlEncodedByteArrayAttribute : ModelBinderAttribute
    {
        /// <summary>
        /// Creates a new instances of <see cref="Base64UrlEncodedByteArrayAttribute"/>.
        /// </summary>
        public Base64UrlEncodedByteArrayAttribute()
        {
            BinderType = typeof(Base64UrlEncodedByteArrayModelBinder);
        }
    }
}
