using System.ComponentModel.DataAnnotations;
using System.Reflection;

using FGS.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace FGS.AspNetCore.Mvc.ModelBinding.Validation
{
    /// <remarks>Taken and modified from: https://github.com/rpgkaiser/FoolProof.Core/blob/268c8afc60dc020089ea03920b1499162b0987b4/FoolProof.Core/Utilities/ValidationAdapter.cs.</remarks>
    public class RequiredIfTrueValidationAdapter : AttributeAdapterBase<RequiredIfTrueAttribute>
    {
        public RequiredIfTrueValidationAdapter(RequiredIfTrueAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            var otherPropertyInfo = context.ModelMetadata.ContainerType.GetProperty(Attribute.DependentProperty);

            var displayName = GetMetaDataDisplayName(otherPropertyInfo);
            if (displayName != null)
                Attribute.DependentPropertyDisplayName = displayName;

#pragma warning disable CA1308 // Normalize strings to uppercase
            var validName = RequiredIfTrueAttribute.ClientTypeName.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

            // Add validation rule attributes
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, $"data-val-{validName}", GetErrorMessage(context));

            // Add validation params attributes
            foreach (var validationParam in Attribute.ClientValidationParameters)
            {
#pragma warning disable CA1308 // Normalize strings to uppercase
                var validationKey = $"data-val-{validName}-{validationParam.Key.ToLowerInvariant()}";
#pragma warning restore CA1308 // Normalize strings to uppercase
                var validationValue = validationParam.Value != null && validationParam.Value.GetType() != typeof(string)
#if NET472 || NETSTANDARD2_0
                    ? Newtonsoft.Json.JsonConvert.SerializeObject(validationParam.Value)
#elif NETCOREAPP3_0
                    ? System.Text.Json.JsonSerializer.Serialize<object>(validationParam.Value)
#endif
                    : validationParam.Value as string;

                MergeAttribute(context.Attributes, validationKey, validationValue);
            }
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }

        private static string GetAttributeDisplayName(ICustomAttributeProvider property)
        {
            var attributes = property.GetCustomAttributes(typeof(DisplayAttribute), true);

            return attributes.Length == 0
                    ? null
                    : (attributes[0] as DisplayAttribute)?.GetName();
        }

        private static string GetMetaDataDisplayName(MemberInfo property)
        {
            var attributes = property.DeclaringType.GetCustomAttributes(typeof(ModelMetadataTypeAttribute), true);

            if (attributes.Length == 0)
                return GetAttributeDisplayName(property);

            var metadataAttribute = attributes[0] as ModelMetadataTypeAttribute;

            var metaProperty = metadataAttribute.MetadataType.GetProperty(property.Name);

            return metaProperty == null
                    ? null
                    : GetAttributeDisplayName(metaProperty);
        }
    }
}
