using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Ninject;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using Ninject.Web.WebApi.Filter;
using Ninject.Web.WebApi.FilterBindingSyntax;

namespace FGS.Pump.Extensions.DI.WebApi
{
    /// <summary>
    /// An adapter on <see cref="IBindingNamedWithOrOnSyntax&lt;T&gt;"/> that let's us pretend like it is <see cref="IFilterBindingWithOrOnSyntax&lt;IFilter&gt;"/>.
    /// </summary>
    /// <remarks>Taken from (and afterwards modified) a decompiled version (likely predating) https://github.com/ninject/Ninject.Web.Mvc/blob/206f1d6916dc47e808cde9bac75e69d3272e9f1b/mvc3/src/Ninject.Web.Mvc/FilterBindingSyntax/FilterFilterBindingBuilder.cs </remarks>
    public class DecoraptedApiFilterBindingWithSyntaxAdapter<TFilter, TFilterType> : IFilterBindingWithOrOnSyntax<TFilterType>
        where TFilter : TFilterType
        where TFilterType : IFilter
    {
        private readonly IBindingNamedWithOrOnSyntax<TFilter> _filterBindingSyntax;

        public DecoraptedApiFilterBindingWithSyntaxAdapter(IBindingNamedWithOrOnSyntax<TFilter> filterBindingSyntax)
        {
            _filterBindingSyntax = filterBindingSyntax;
        }

        public IBindingConfiguration BindingConfiguration => _filterBindingSyntax.BindingConfiguration;

        public IKernel Kernel => _filterBindingSyntax.Kernel;

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param><param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithConstructorArgument(string name, object value)
        {
            _filterBindingSyntax.WithConstructorArgument(name, value);
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param><param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            _filterBindingSyntax.WithConstructorArgument(name, callback);
            return this;
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param><param name="value">The value for the property.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithPropertyValue(string name, object value)
        {
            _filterBindingSyntax.WithPropertyValue(name, value);
            return this;
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param><param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithPropertyValue(string name, Func<IContext, object> callback)
        {
            _filterBindingSyntax.WithPropertyValue(name, callback);
            return this;
        }

        /// <summary>
        /// Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithParameter(IParameter parameter)
        {
            _filterBindingSyntax.WithParameter(parameter);
            return this;
        }

        /// <summary>
        /// Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param><param name="value">The metadata value.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithMetadata(string key, object value)
        {
            _filterBindingSyntax.WithMetadata(key, value);
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param><param name="callback">The callback.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithConstructorArgument(string name, Func<IContext, HttpConfiguration, HttpActionDescriptor, object> callback)
        {
            var callbackAdapter = (Func<IContext, object>)(ctx =>
                {
                    var contextParameter = GetAncestorFilterContextParameter(ctx);
                    return callback(ctx, contextParameter.HttpConfiguration, contextParameter.ActionDescriptor);
                });
            return WithConstructorArgument(name, callbackAdapter);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        ///             The value is retrieved from an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam><param name="name">The name of the argument to override.</param><param name="callback">The callback.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithConstructorArgumentFromActionAttribute<TAttribute>(string name, Func<TAttribute, object> callback)
            where TAttribute : class
        {
            return WithConstructorArgument(name, (ctx, controllerContext, actionDescriptor) => callback(actionDescriptor.GetCustomAttributes<TAttribute>().Single()));
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        ///             The value is retrieved from an attribute on the controller of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam><param name="name">The name of the argument to override.</param><param name="callback">The callback.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithConstructorArgumentFromControllerAttribute<TAttribute>(string name, Func<TAttribute, object> callback)
            where TAttribute : class
        {
            return WithConstructorArgument(name, (ctx, controllerContext, actionDescriptor) => callback(actionDescriptor.ControllerDescriptor.GetCustomAttributes<TAttribute>().Single()));
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param><param name="callback">The cllback to retrieve the value.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithPropertyValue(string name, Func<IContext, HttpConfiguration, HttpActionDescriptor, object> callback)
        {
            var callbackAdapter = (Func<IContext, object>)(ctx =>
                {
                    var contextParameter = GetAncestorFilterContextParameter(ctx);
                    return callback(ctx, contextParameter.HttpConfiguration, contextParameter.ActionDescriptor);
                });
            return WithPropertyValue(name, callbackAdapter);
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        ///             The value is retrieved from an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam><param name="name">The name of the property to override.</param><param name="callback">The cllback to retrieve the value.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithPropertyValueFromActionAttribute<TAttribute>(string name, Func<TAttribute, object> callback)
            where TAttribute : class
        {
            return WithPropertyValue(name, (ctx, controllerContext, actionDescriptor) => callback(actionDescriptor.GetCustomAttributes<TAttribute>().Single()));
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        ///             The value is retrieved from an attribute on the controller of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam><param name="name">The name of the property to override.</param><param name="callback">The cllback to retrieve the value.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingWithOrOnSyntax<TFilterType> WithPropertyValueFromControllerAttribute<TAttribute>(string name, Func<TAttribute, object> callback)
            where TAttribute : class
        {
            return WithPropertyValue(name, (ctx, controllerContext, actionDescriptor) => callback(actionDescriptor.ControllerDescriptor.GetCustomAttributes<TAttribute>().Single()));
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingOnSyntax<TFilterType> OnActivation(Action<TFilterType> action)
        {
            _filterBindingSyntax.OnActivation(action);
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingOnSyntax<TFilterType> OnActivation(Action<IContext, TFilterType> action)
        {
            _filterBindingSyntax.OnActivation(action);
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingOnSyntax<TFilterType> OnDeactivation(Action<IContext, TFilterType> action)
        {
            _filterBindingSyntax.OnDeactivation(action);
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingOnSyntax<TFilterType> OnDeactivation(Action<TFilterType> action)
        {
            _filterBindingSyntax.OnDeactivation(action);
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax to define more information
        /// </returns>
        public IFilterBindingOnSyntax<TFilterType> OnActivation(Action<IContext, HttpConfiguration, HttpActionDescriptor, TFilterType> action)
        {
            OnActivation((ctx, instance) =>
                {
                    var contextParameter = GetAncestorFilterContextParameter(ctx);
                    action(ctx, contextParameter.HttpConfiguration, contextParameter.ActionDescriptor, instance);
                });
            return this;
        }

        /// <summary>
        /// Gets the filter context parameter.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <returns>
        /// The filter context parameter from the context parameters.
        /// </returns>
        private static FilterContextParameter GetAncestorFilterContextParameter(IContext ctx)
        {
            if (ctx.Request.ParentContext != null)
            {
                var result = ctx.Parameters.OfType<FilterContextParameter>().SingleOrDefault();
                return result ?? GetAncestorFilterContextParameter(ctx.Request.ParentContext);
            }

            return ctx.Parameters.OfType<FilterContextParameter>().Single();
        }
    }
}