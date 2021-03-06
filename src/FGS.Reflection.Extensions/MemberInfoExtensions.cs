using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FGS.Reflection.Extensions
{
    /// <summary>
    /// Provides functionality to assist with reading information about the members of types.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/ninject/Ninject/blob/cc00946b1484db3c8d1c80c0c44e91beabc6b5be/src/Ninject/Infrastructure/Language/ExtensionsForMemberInfo.cs </remarks>
    /// <remarks>Other parts taken and modified from: https://github.com/autofac/Autofac/blob/d5fb10034f14564f2d3d59bc70ffd793161b677e/src/Autofac/Util/ReflectionExtensions.cs </remarks>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Determines whether the specified member has a given type of attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute that is sought.</typeparam>
        /// <param name="member">The member being inquired about.</param>
        /// <returns><see langword="true"/> if the specified member has the attribute; otherwise <see langword="false"/>.</returns>
        public static bool HasAttribute<T>(this MemberInfo member)
        {
            return member.HasAttribute(typeof(T));
        }

        /// <summary>
        /// Determines whether the specified member has a given type of attribute.
        /// </summary>
        /// <param name="member">The member being inquired about.</param>
        /// <param name="type">The type of the attribute that is sought.</param>
        /// <returns><see langword="true"/> if the specified member has the attribute; otherwise <see langword="false"/>.</returns>
        public static bool HasAttribute(this MemberInfo member, Type type)
        {
            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                return IsDefined(propertyInfo, type, true);
            }

            return member.IsDefined(type, true);
        }

        private static PropertyInfo GetPropertyFromDeclaredType(this MemberInfo memberInfo, PropertyInfo propertyDefinition)
        {
            return memberInfo.DeclaringType.GetRuntimeProperties().FirstOrDefault(
                p => p.Name == propertyDefinition.Name &&
                    !p.GetMethod.IsStatic &&
                     p.PropertyType == propertyDefinition.PropertyType &&
                     p.GetIndexParameters().SequenceEqual(propertyDefinition.GetIndexParameters(), new ParameterInfoEqualityComparer()));
        }

        private class ParameterInfoEqualityComparer : IEqualityComparer<ParameterInfo>
        {
            public bool Equals(ParameterInfo x, ParameterInfo y)
            {
                return x.Position == y.Position && x.ParameterType == y.ParameterType;
            }

            public int GetHashCode(ParameterInfo obj)
            {
                return obj.Position.GetHashCode() ^ obj.ParameterType.GetHashCode();
            }
        }

        /// <summary>
        /// Determines whether <paramref nam="propertyInfo"/> represents a private property.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns><see langword="true"/> if the specified property is private; otherwise <see langword="false"/>.</returns>
        public static bool IsPrivate(this PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetMethod;
            var setMethod = propertyInfo.SetMethod;

            return (getMethod == null || getMethod.IsPrivate) && (setMethod == null || setMethod.IsPrivate);
        }

        /// <summary>
        /// Maps from a property-set-value parameter to the declaring property.
        /// </summary>
        /// <param name="pi">Parameter to the property setter.</param>
        /// <param name="prop">The property info on which the setter is specified.</param>
        /// <returns><see langword="true"/> if the parameter is a property setter; otherwise <see langword="false"/>.</returns>
        public static bool TryGetDeclaringProperty(this ParameterInfo pi, out PropertyInfo prop)
        {
            var mi = pi.Member as MethodInfo;
            if (mi != null && mi.IsSpecialName && mi.Name.StartsWith("set_", StringComparison.Ordinal) && mi.DeclaringType != null)
            {
                prop = mi.DeclaringType.GetTypeInfo().GetDeclaredProperty(mi.Name.Substring(4));
                return true;
            }

            prop = null;
            return false;
        }

        private static PropertyInfo GetParentDefinition(PropertyInfo property)
        {
            var propertyMethod = property.GetMethod ?? property.SetMethod;

            if (propertyMethod != null)
            {
                propertyMethod = propertyMethod.GetParentDefinition();
                if (propertyMethod != null)
                {
                    return propertyMethod.GetPropertyFromDeclaredType(property);
                }
            }

            return null;
        }

        private static MethodInfo GetParentDefinition(this MethodInfo method)
        {
            var baseDefinition = method.GetRuntimeBaseDefinition();

            var type = method.DeclaringType.GetTypeInfo().BaseType;

            MethodInfo result = null;
            while (result == null && type != null)
            {
                result = type.GetRuntimeMethods().SingleOrDefault(m => !m.IsStatic && m.GetRuntimeBaseDefinition().Equals(baseDefinition));
                type = type.GetTypeInfo().BaseType;
            }

            return result;
        }

        private static bool IsDefined(PropertyInfo element, Type attributeType, bool inherit)
        {
            if (element.IsDefined(attributeType, false))
            {
                return true;
            }

            if (inherit)
            {
                if (!InternalGetAttributeUsage(attributeType).Inherited)
                {
                    return false;
                }

                for (var info = GetParentDefinition(element);
                     info != null;
                     info = GetParentDefinition(info))
                {
                    if (info.IsDefined(attributeType, false))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static IEnumerable<Attribute> GetCustomAttributes(PropertyInfo propertyInfo, Type attributeType, bool inherit)
        {
            if (inherit)
            {
                if (InternalGetAttributeUsage(attributeType).Inherited)
                {
                    var attributes = new List<Attribute>();

                    var attributeUsages = new Dictionary<Type, bool>();
                    attributes.AddRange(propertyInfo.GetCustomAttributes(attributeType, false).Cast<Attribute>());
                    for (var info = GetParentDefinition(propertyInfo);
                         info != null;
                         info = GetParentDefinition(info))
                    {
                        var customAttributes = info.GetCustomAttributes(attributeType, false).Cast<Attribute>();
                        AddAttributes(attributes, customAttributes, attributeUsages);
                    }

                    return attributes;
                }
            }

            return propertyInfo.GetCustomAttributes(attributeType, inherit).Cast<Attribute>();
        }

        private static void AddAttributes(ICollection<Attribute> attributes, IEnumerable<Attribute> customAttributes, IDictionary<Type, bool> attributeUsages)
        {
            foreach (var attribute in customAttributes)
            {
                var type = attribute.GetType();
                if (!attributeUsages.ContainsKey(type))
                {
                    attributeUsages[type] = InternalGetAttributeUsage(type).Inherited;
                }

                if (attributeUsages[type])
                {
                    attributes.Add(attribute);
                }
            }
        }

        private static AttributeUsageAttribute InternalGetAttributeUsage(Type type)
        {
            var customAttributes = type.GetTypeInfo().GetCustomAttributes(typeof(AttributeUsageAttribute), true);
            return (AttributeUsageAttribute)customAttributes.First();
        }
    }
}
