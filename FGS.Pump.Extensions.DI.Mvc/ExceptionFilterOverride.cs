using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace FGS.Pump.Extensions.DI.Mvc
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/ac6478034bdd32938fdd6b4923519df95f98ab18/src/Autofac.Integration.Mvc/ExceptionFilterOverride.cs </remarks>
    internal class ExceptionFilterOverride : ExceptionFilterReflectiveFacade, IOverrideFilter
    {
        public ExceptionFilterOverride(IExceptionFilter wrappedFilter)
            : base(wrappedFilter)
        {
        }

        public Type FiltersToOverride => typeof(IExceptionFilter);
    }
}