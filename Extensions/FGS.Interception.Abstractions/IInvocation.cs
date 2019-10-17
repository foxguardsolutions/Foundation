namespace FGS.Interception.Abstractions
{
    /// <summary>
    /// Represents a method invocation that has been intercepted.
    /// </summary>
    public interface IInvocation : IInvocationCommon
    {
        /// <summary>
        /// Execute the intercepted method invocation.
        /// </summary>
        /// <remarks>To access the invocation return value, use the ReturnValue property.</remarks>
        void Proceed();
    }
}
