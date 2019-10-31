using System.Linq;

using AutoFixture;
using AutoFixture.AutoMoq;

namespace FGS.Tests.Support
{
    /// <summary>
    /// Provides a factory method for creating a configured <see cref="Fixture"/>.
    /// </summary>
    public static class AutoFixtureFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="Fixture"/>, which has been configured with <see cref="AutoMoqCustomization"/>,
        /// had <see cref="ThrowingRecursionBehavior"/> removed, and had <see cref="OmitOnRecursionBehavior"/> added to it.
        /// </summary>
        /// <returns>A configured instance of <see cref="Fixture"/>.</returns>
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
