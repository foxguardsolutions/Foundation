using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc.Tests.TestTypes
{
    /// <remarks>Taken from: https://github.com/autofac/Autofac.Mvc/blob/e26ce3fe9ccc639f1349bcd8aee8e6e4ee066346/test/Autofac.Integration.Mvc.Test/TestTypes.cs </remarks>
    public class TestControllerB : TestControllerA
    {
        public override ActionResult Action1(string value)
        {
            return new EmptyResult();
        }

        public override ActionResult Action2(int value)
        {
            return new EmptyResult();
        }
    }
}