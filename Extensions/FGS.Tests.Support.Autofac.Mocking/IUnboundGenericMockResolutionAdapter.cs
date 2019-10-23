using Moq;

namespace FGS.Tests.Support.Autofac.Mocking
{
    public interface IUnboundGenericMockResolutionAdapter
    {
        Mock Mock { get; }
    }
}
