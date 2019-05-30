using System.Linq;

using AutoFixture;
using AutoFixture.AutoMoq;

namespace FGS.Tests.Support
{
    public static class AutoFixtureFactory
    {
        public static Fixture Create()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.Remove(fixture.Behaviors.Single(b => b is ThrowingRecursionBehavior));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}
