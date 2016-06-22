using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

using FGS.Pump.Extensions.Lifetime;

using Ninject.Activation;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using Ninject.Planning.Targets;

namespace FGS.Pump.Extensions.DI
{
    /// <remarks>Taken and modified from: https://github.com/foxguardsolutions/psa/blob/38ad1a86f24152d31559249d45a5b5e3b9eb22a0/CPSA/IoC/OwnedLifetimeModule.cs </remarks>>
    public class OwnedLifetimeModule : NinjectModule
    {
        public override void Load()
        {
            // We have to make sure the compiler knows we are implicitly dependent upon this Ninject extension library,
            // which Ninject loads dynamically at runtime.
            // ReSharper disable once UnusedVariable
            var ignoreMe = typeof(FuncModule);

            Bind(typeof(Owned<>)).ToMethod(UntypedOwnedFactory);
        }

        /// <remarks>To support binding <see cref="Owned{T}"/> properly, we have to start out by binding the open generic of it to a factory that isn't strongly typed.</remarks>
        private static object UntypedOwnedFactory(IContext context)
        {
            // Then we use reflection to defer implementation of the binding's factory to a method that is more strongly-typed.
            var ownedsInnerValueType = context.Request.Service.GenericTypeArguments[0];
            return typeof(OwnedLifetimeModule).GetMethod("OwnedFactory", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(ownedsInnerValueType)
                    .Invoke(null, new object[] { context });
        }

        // ReSharper disable once UnusedMember.Local

        /// <summary>
        /// Creates an instance of <see cref="Owned{T}"/> from the current Ninject context.
        /// </summary>
        /// <remarks>
        /// Here be dragons!
        /// </remarks>
        private static Owned<T> OwnedFactory<T>(IContext context)
        {
            /* When an Owned<T> is requested from DI, it is almost always in the form of Func<… Owned<T>> .
               We need to make sure that the parameters that the developer provides to that Func are still available for the creation
               of instance T that the Owned<> wraps. */

            var originalFactoryExplicitParameters = context.Parameters.OfType<FuncConstructorArgument>().ToArray();
            var originalFactoryExplicitParameterValues = Enumerable.ToArray<object>(originalFactoryExplicitParameters.Select(x => x.GetValue(context, default(ITarget))));
            var originalFactoryImplicitParameters = context.Parameters.Except(originalFactoryExplicitParameters).ToArray();

            var innerFactorySignature = Enumerable.Concat(originalFactoryExplicitParameters.Select(x => x.ArgumentType), new[] { typeof(T) }).ToArray();
            var innerFactoryType = FuncTypesByGenericArity[innerFactorySignature.Length].MakeGenericType(innerFactorySignature);

            var innerFactory = (Delegate)context.ContextPreservingGet(innerFactoryType, originalFactoryImplicitParameters);
            Func<T> invokeInnerFactory = () => (T)innerFactory.DynamicInvoke(originalFactoryExplicitParameterValues);

            var actualValue = invokeInnerFactory();
            return new Owned<T>(actualValue, new OnDispose(() => context.Kernel.Release(actualValue)));
        }

        private static readonly IDictionary<int, Type> FuncTypesByGenericArity = new ReadOnlyDictionary<int, Type>(new Dictionary<int, Type>()
                                                  {
                                                      { 1, typeof(Func<>) },
                                                      { 2, typeof(Func<,>) },
                                                      { 3, typeof(Func<,,>) },
                                                      { 4, typeof(Func<,,,>) },
                                                      { 5, typeof(Func<,,,,>) },
                                                      { 6, typeof(Func<,,,,,>) },
                                                      { 7, typeof(Func<,,,,,,>) },
                                                      { 8, typeof(Func<,,,,,,,>) },
                                                      { 9, typeof(Func<,,,,,,,,>) },
                                                      { 10, typeof(Func<,,,,,,,,,>) },
                                                      { 11, typeof(Func<,,,,,,,,,,>) },
                                                      { 12, typeof(Func<,,,,,,,,,,,>) },
                                                      { 13, typeof(Func<,,,,,,,,,,,,>) },
                                                      { 14, typeof(Func<,,,,,,,,,,,,,>) },
                                                      { 15, typeof(Func<,,,,,,,,,,,,,,>) },
                                                      { 16, typeof(Func<,,,,,,,,,,,,,,,>) },
                                                      { 17, typeof(Func<,,,,,,,,,,,,,,,,>) }
                                                  });
    }
}
