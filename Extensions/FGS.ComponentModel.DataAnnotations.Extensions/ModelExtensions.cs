using System;
using System.Linq.Expressions;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FGS.ComponentModel.DataAnnotations.Extensions
{
    public static class ModelExtensions
    {
        public static string GetDisplayName<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> expression)
        {
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            return modelMetadataProvider.GetModelExplorerForType(typeof(TModel), model).GetExplorerForExpression(typeof(TProperty), expression).GetSimpleDisplayText();
        }
    }
}
