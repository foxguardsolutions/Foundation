namespace FGS.Autofac.DynamicScoping.Abstractions
{
    public enum Scope
    {
        PerDependency,
        PerLifetimeScope,
        PerRequest,
        Singleton
    }
}