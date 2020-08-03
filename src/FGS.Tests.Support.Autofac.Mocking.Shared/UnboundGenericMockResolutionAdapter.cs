using Moq;

namespace FGS.Tests.Support.Autofac.Mocking
{
    internal class UnboundGenericMockResolutionAdapter<T> : IUnboundGenericMockResolutionAdapter
        where T : class
    {
        private readonly Mock<T> _mock;

        internal UnboundGenericMockResolutionAdapter(Mock<T> mock)
        {
            _mock = mock;
        }

        public Mock Mock => _mock;
    }
}
