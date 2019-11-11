using System.Collections.Generic;
using System.Threading.Tasks;

namespace FGS.Autofac.Interception.DynamicProxy.Tests
{
    public class InterceptionSubjectInnerImplementationDecorator : IInterceptionTestSubject
    {
        private readonly IInterceptionTestSubject _decorated;

        public InterceptionSubjectInnerImplementationDecorator(IInterceptionTestSubject decorated)
        {
            _decorated = decorated;
        }

        [InterceptForTest]
        public virtual void ExecuteVoid(int valueArgument, string referenceArgument)
        {
            _decorated.ExecuteVoid(valueArgument, referenceArgument);
        }

        [InterceptForTest]
        public virtual bool ExecuteReturnValue(int valueArgument, string referenceArgument)
        {
            return _decorated.ExecuteReturnValue(valueArgument, referenceArgument);
        }

        [InterceptForTest]
        public virtual IEnumerable<bool> ExecuteReturnReference(int valueArgument, string referenceArgument)
        {
            return _decorated.ExecuteReturnReference(valueArgument, referenceArgument);
        }

        [InterceptForTest]
        public virtual Task ExecuteReturnAsync(int valueArgument, string referenceArgument)
        {
            return _decorated.ExecuteReturnAsync(valueArgument, referenceArgument);
        }

        [InterceptForTest]
        public virtual Task<bool> ExecuteReturnAsyncValue(int valueArgument, string referenceArgument)
        {
            return _decorated.ExecuteReturnAsyncValue(valueArgument, referenceArgument);
        }

        [InterceptForTest]
        public virtual Task<IEnumerable<bool>> ExecuteReturnAsyncReference(int valueArgument, string referenceArgument)
        {
            return _decorated.ExecuteReturnAsyncReference(valueArgument, referenceArgument);
        }
    }
}