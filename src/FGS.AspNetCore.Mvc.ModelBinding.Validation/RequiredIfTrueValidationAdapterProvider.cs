using System.ComponentModel.DataAnnotations;

using FGS.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace FGS.AspNetCore.Mvc.ModelBinding.Validation
{
    /// <summary>
    /// Wire this into dependency injection in order for <see cref="RequiredIfTrueAttribute"/> to be adapted to have its correct implementation.
    /// </summary>
    /// <example>
    /// <code>
    ///   services.AddSingleton&lt;IValidationAttributeAdapterProvider, RequiredIfTrueValidationAdapterProvider&gt;();
    /// </code>
    /// </example>
    /// <remarks>Taken and modified from: https://github.com/rpgkaiser/FoolProof.Core/blob/268c8afc60dc020089ea03920b1499162b0987b4/FoolProof.Core/Utilities/ValidationAdapterProvider.cs.</remarks>
    public class RequiredIfTrueValidationAdapterProvider : ValidationAttributeAdapterProvider, IValidationAttributeAdapterProvider
    {
        IAttributeAdapter IValidationAttributeAdapterProvider.GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            IAttributeAdapter adapter;
            if (attribute is RequiredIfTrueAttribute requiredIfTrueAttribute)
                adapter = new RequiredIfTrueValidationAdapter(requiredIfTrueAttribute, stringLocalizer);
            else
                adapter = GetAttributeAdapter(attribute, stringLocalizer);

            return adapter;
        }
    }
}
