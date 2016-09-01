using System.Collections.Generic;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/TestTypes.cs </remarks>
    public class TestControllerA : TestController
    {
        public override IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }
    }
}