using System.Threading.Tasks;

using FGS.Primitives.Extensions;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FGS.AspNetCore.Mvc.ModelBinding
{
    public class StringCleanupModelBinder : IModelBinder
    {
        private readonly char[] _charsToTrim = { '\'', '\"', '`' };

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
