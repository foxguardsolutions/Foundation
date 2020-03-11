using Moq;

namespace FGS.Tests.Support.Autofac.Mocking
{
    internal interface IUnboundGenericMockResolutionAdapter
    {
        Mock Mock { get; }
    }
}
