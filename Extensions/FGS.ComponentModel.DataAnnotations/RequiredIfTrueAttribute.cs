using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FGS.ComponentModel.DataAnnotations
{
    /// <remarks>Taken and modified from: https://github.com/StefanKern/foolproof/blob/1996fa42dbbcad228c6bca7f790c905252bb4514/Foolproof/RequiredIf.cs.</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfTrueAttribute : ValidationAttribute
    {
        public string DependentProperty { get; }
        public string DependentPropertyDisplayName { get; set; }

        public const string ClientTypeName = "RequiredIf";
        private const bool DependentValue = true;
        private const string DefaultErrorMessage = "{0} is required due to {1} being equal to {2}";

        public RequiredIfTrueAttribute(string dependentProperty)
        {
            DependentProperty = dependentProperty;
        }

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

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var validate = IsValid(value, validationContext.ObjectInstance);
            return validate
                    ? ValidationResult.Success
                    : new ValidationResult(this.ErrorMessage, new[] { validationContext.MemberName });
        }

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

        public Dictionary<string, object> ClientValidationParameters =>
            new Dictionary<string, object>()
            {
                { "DependentProperty", DependentProperty },
                { "Operator", "EqualTo" },
                { "DependentValue", DependentValue },
            };
    }
}
