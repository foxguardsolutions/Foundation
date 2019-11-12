using System;

using Microsoft.AspNetCore.Mvc;

namespace FGS.AspNetCore.Mvc.ModelBinding
{
    /// <summary>
    /// Indicates that the given model member should be bound with <see cref="StringCleanupModelBinder"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class StringCleanupAttribute : ModelBinderAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="StringCleanupAttribute"/>.
        /// </summary>
        public StringCleanupAttribute()
        {
            BinderType = typeof(StringCleanupModelBinder);
        }
    }
}
