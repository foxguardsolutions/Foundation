using System;

using Microsoft.AspNetCore.Mvc;

namespace FGS.AspNetCore.Mvc.ModelBinding
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class StringCleanupAttribute : ModelBinderAttribute
    {
        public StringCleanupAttribute()
        {
            BinderType = typeof(StringCleanupModelBinder);
        }
    }
}
