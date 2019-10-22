using System.Collections.Generic;
using System.Threading.Tasks;

namespace FGS.Autofac.Interception.DynamicProxy.Tests
{
    public interface IInterceptionTestSubject
    {
        void ExecuteVoid(int valueArgument, string referenceArgument);
        bool ExecuteReturnValue(int valueArgument, string referenceArgument);
        IEnumerable<bool> ExecuteReturnReference(int valueArgument, string referenceArgument);
        Task ExecuteReturnAsync(int valueArgument, string referenceArgument);
        Task<bool> ExecuteReturnAsyncValue(int valueArgument, string referenceArgument);
        Task<IEnumerable<bool>> ExecuteReturnAsyncReference(int valueArgument, string referenceArgument);
    }
}