using System.Reflection;
using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc.Tests.TestTypes
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/e26ce3fe9ccc639f1349bcd8aee8e6e4ee066346/test/Autofac.Integration.Mvc.Test/TestTypes.cs </remarks>
    public class TestController : Controller
    {
        public object Dependency { get; set; }

        public virtual ActionResult Action1(string value)
        {
            return new EmptyResult();
        }

        public virtual ActionResult Action2(int value)
        {
            return new EmptyResult();
        }

        public static MethodInfo GetAction1MethodInfo<T>()
            where T : TestController
        {
            return typeof(T).GetMethod("Action1");
        }
    }
}