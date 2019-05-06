namespace FGS.Pump.Extensions.DI.Interception
{
    public interface IInvocationCommon
    {
        object GetArgumentValue(int index);
        System.Reflection.MethodInfo GetConcreteMethod();
        System.Reflection.MethodInfo GetConcreteMethodInvocationTarget();
        void SetArgumentValue(int index, object value);
        object[] Arguments { get; }
        System.Type[] GenericArguments { get; }
        object InvocationTarget { get; }
        System.Reflection.MethodInfo Method { get; }
        System.Reflection.MethodInfo MethodInvocationTarget { get; }
        object Proxy { get; }
        object ReturnValue { get; set; }
        System.Type TargetType { get; }
    }
}
