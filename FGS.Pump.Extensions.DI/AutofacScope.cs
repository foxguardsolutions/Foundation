namespace FGS.Pump.Extensions.DI
{
    public enum AutofacScope
    {
        PerDependency,
        PerLifetimeScope,
        Transient,
        Parent,
        PerRequest,
        Singleton
    }
}
