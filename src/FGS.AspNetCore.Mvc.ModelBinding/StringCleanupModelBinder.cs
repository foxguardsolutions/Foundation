using System.Threading.Tasks;

using FGS.Primitives.Extensions;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FGS.AspNetCore.Mvc.ModelBinding
{
    /// <summary>
    /// An implementation of <see cref="IModelBinder"/> that trims whitespace and pairs of various kinds of
    /// quotation marks from the ends of a text member.
    /// </summary>
    public class StringCleanupModelBinder : IModelBinder
    {
        private readonly char[] _charsToTrim = { '\'', '\"', '`' };

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(string))
            {
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
                if (!string.IsNullOrEmpty(value))
                {
                    value = value.TrimWhitespaceAndCharacters(_charsToTrim);
                }

                bindingContext.Result = ModelBindingResult.Success(value);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}
