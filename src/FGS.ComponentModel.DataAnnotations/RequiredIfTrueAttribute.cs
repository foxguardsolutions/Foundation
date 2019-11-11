using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FGS.ComponentModel.DataAnnotations
{
    /// <summary>Marks a member as possibly being required, dependent on the value of a sibling member.</summary>
    /// <remarks>Taken and modified from: https://github.com/StefanKern/foolproof/blob/1996fa42dbbcad228c6bca7f790c905252bb4514/Foolproof/RequiredIf.cs.</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfTrueAttribute : ValidationAttribute
    {
        /// <summary>
        /// Gets the name of the sibling property which must be <see langword="true"/> for the annotated member to be required.
        /// </summary>
        public string DependentProperty { get; }

        /// <summary>
        /// Gets or sets the human-friendly way to render the name of the dependent sibling property in generated validation messages.
        /// </summary>
        public string DependentPropertyDisplayName { get; set; }

        public const string ClientTypeName = "RequiredIf";
        private const bool DependentValue = true;
        private const string DefaultErrorMessage = "{0} is required due to {1} being equal to {2}";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredIfTrueAttribute"/> class.
        /// </summary>
        /// <param name="dependentProperty">The name of the sibling property which must be <see langword="true"/> for the annotated member to be required.</param>
        public RequiredIfTrueAttribute(string dependentProperty)
        {
            DependentProperty = dependentProperty;
        }

        /// <inheritdoc />
        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrEmpty(ErrorMessageResourceName) && string.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = DefaultErrorMessage;

            return string.Format(ErrorMessageString, name, DependentProperty, DependentValue);
        }

        private object GetDependentPropertyValue(object container)
        {
            var currentType = container.GetType();
            var value = container;

            foreach (var propertyName in DependentProperty.Split('.'))
            {
                var property = currentType.GetProperty(propertyName);
                value = property.GetValue(value, null);
                currentType = property.PropertyType;
            }

            return value;
        }

        /// <inheritdoc />
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var validate = IsValid(value, validationContext.ObjectInstance);
            return validate
                    ? ValidationResult.Success
                    : new ValidationResult(this.ErrorMessage, new[] { validationContext.MemberName });
        }

        /// <summary>
        /// Determines whether or not the given <paramref name="value"/> is valid based on the given <paramref name="dependentPropertyValue"/>.
        /// </summary>
        /// <param name="value">The value of the annotated member being validated.</param>
        /// <param name="dependentPropertyValue">The value of the sibling member that factors into validation.</param>
        protected internal static bool IsValidGivenDependentPropertyValue(object value, object dependentPropertyValue)
        {
            if (value != null) return true;

            return
                (bool)dependentPropertyValue == DependentValue
                ? value != null
                : true;
        }

        private bool IsValid(object value, object container) =>
            IsValidGivenDependentPropertyValue(value, GetDependentPropertyValue(container));

        /// <summary>
        /// Gets data exposed for use by adapters that will relay it to client-side validation frameworks.
        /// </summary>
        public Dictionary<string, object> ClientValidationParameters =>
            new Dictionary<string, object>()
            {
                { "DependentProperty", DependentProperty },
                { "Operator", "EqualTo" },
                { "DependentValue", DependentValue },
            };
    }
}
