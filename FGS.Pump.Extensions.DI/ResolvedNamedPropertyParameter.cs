using System;
using System.Reflection;

using Autofac;

namespace FGS.Pump.Extensions.DI
{
    /// <remarks>Based on:
    ///     https://github.com/autofac/Autofac/blob/d5fb10034f14564f2d3d59bc70ffd793161b677e/src/Autofac/Core/ResolvedParameter.cs
    /// and https://github.com/autofac/Autofac/blob/d5fb10034f14564f2d3d59bc70ffd793161b677e/src/Autofac/Core/NamedPropertyParameter.cs </remarks>
    public class ResolvedNamedPropertyParameter<TPropertyValue> : Autofac.Core.Parameter
    {
        public string Name { get; }

        private readonly Func<IComponentContext, TPropertyValue> _valueAccessor;

        public ResolvedNamedPropertyParameter(string propertyName, Func<IComponentContext, TPropertyValue> valueAccessor)
        {
            Name = propertyName;
            _valueAccessor = valueAccessor;
        }

        public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
        {
            if (!pi.TryGetDeclaringProperty(out var prop) || prop.Name != Name)
            {
                valueProvider = null;
                return false;
            }

            valueProvider = () => _valueAccessor(context);
            return true;
        }
    }
}
