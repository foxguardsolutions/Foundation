using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc.Tests.TestTypes
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/e26ce3fe9ccc639f1349bcd8aee8e6e4ee066346/test/Autofac.Integration.Mvc.Test/TestTypes.cs </remarks>
    public class TestExceptionFilter2 : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
        }
    }
}