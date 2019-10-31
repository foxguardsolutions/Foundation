using System;
using System.ComponentModel;
using System.Linq.Expressions;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FGS.ComponentModel.DataAnnotations.Extensions
{
    /// <summary>
    /// Extends all objects with the ability to retrieve model metadata.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Gets the display name of a model member, typically provided by an appropriate annotation of <see cref="DisplayNameAttribute"/>.
        /// </summary>
        /// <param name="model">The model containing the member whose display name is sought.</param>
        /// <param name="expression">An expression that selects the subject member from the provided <paramref name="model"/>.</param>
        /// <typeparam name="TModel">The type of model containing the member whose display name is sought.</typeparam>
        /// <typeparam name="TProperty">The type of the member whose display name is sought.</typeparam>
        /// <returns>The display name of the indicated model member.</returns>
        public static string GetDisplayName<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> expression)
        {
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            return modelMetadataProvider.GetModelExplorerForType(typeof(TModel), model).GetExplorerForExpression(typeof(TProperty), expression).GetSimpleDisplayText();
        }
    }
}
