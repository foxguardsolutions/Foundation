using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace FGS.Pump.Extensions
{
    public static class ModelExtensions
    {
        public static string GetDisplayName<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expression)
        {
            return ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<TModel>(model)).DisplayName;
        }
    }
}
