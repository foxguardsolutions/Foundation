using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FGS.AspNetCore.Mvc.ModelBinding
{
    public class Base64UrlEncodedByteArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(byte[]))
            {
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue as string;

                try
                {
                    byte[] decodedValue;

                    // Some clients (PAR) use an alternative encoding (see: https://github.com/mono/mono/blob/b7a308f660de8174b64697a422abfc7315d07b8c/mcs/class/System.Web/System.Web/HttpServerUtility.cs )
                    // We special case to remove an extra character from the alternate encoding to prevent ASP.NET Core from throwing an exception.
                    if (value.Length % 4 == 1)
                    {
                        decodedValue = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(value, 0, value.Length - 1);
                    }
                    else
                    {
                        decodedValue = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(value);
                    }

                    bindingContext.Result = ModelBindingResult.Success(decodedValue);
                    return Task.CompletedTask;
                }
                catch (FormatException e)
                {
                    RecordException(bindingContext, e);
                    bindingContext.Result = ModelBindingResult.Failed();
                    return Task.CompletedTask;
                }
            }

            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        private static void RecordException(ModelBindingContext bindingContext, Exception exception)
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, exception, bindingContext.ModelMetadata);
        }
    }
}
