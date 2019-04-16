using System;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FGS.AspNetCore.Mvc.ModelBinding
{
    public class ParseMethodInvokingModelBinder : IModelBinder
    {
        private readonly MethodInfo _methodInfo;

        public ParseMethodInvokingModelBinder(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
        }

        #region Implementation of IModelBinder

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            // This special case is for UI code that, when written, accidentally relied on MVC 5's tendency to coerce failed
            // model parse attempts to `null`.
            if (value == "Nothing selected")
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            try
            {
                bindingContext.Result = ModelBindingResult.Success(_methodInfo.Invoke(null, new[] { value }));
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                if (ShouldExceptionBeRethrown(e))
                    throw;

                var exceptionToRecord = GetExceptionToRecord(e);
                RecordException(bindingContext, exceptionToRecord);

                bindingContext.Result = ModelBindingResult.Failed();

                return Task.CompletedTask;
            }
        }

        #endregion

        private static bool ShouldExceptionBeRethrown(Exception e)
        {
            if (e is TargetInvocationException)
                return ShouldExceptionBeRethrown(e.InnerException);

            return !(e is FormatException);
        }

        private static Exception GetExceptionToRecord(Exception e)
        {
            if (e is TargetInvocationException)
                return GetExceptionToRecord(e.InnerException);

            return e;
        }

        private static void RecordException(ModelBindingContext bindingContext, Exception exception)
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, exception, bindingContext.ModelMetadata);
        }
    }
}
