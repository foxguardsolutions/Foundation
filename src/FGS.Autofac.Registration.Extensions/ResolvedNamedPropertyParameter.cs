using System;
using System.Reflection;

using Autofac;
using Autofac.Core;

using FGS.Reflection.Extensions;

namespace FGS.Autofac.Registration.Extensions
{
    internal sealed class ResolvedNamedPropertyParameter<TPropertyValue> : Parameter
    {
        private readonly string _name;
        private readonly Func<IComponentContext, TPropertyValue> _valueAccessor;

        internal ResolvedNamedPropertyParameter(string propertyName, Func<IComponentContext, TPropertyValue> valueAccessor)
        {
            _name = propertyName;
            _valueAccessor = valueAccessor;
        }

        public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
        {
            if (!pi.TryGetDeclaringProperty(out var prop) || prop.Name != _name)
            {
                valueProvider = null;
                return false;
            }

            valueProvider = () => _valueAccessor(context);
            return true;
        }
    }
}
