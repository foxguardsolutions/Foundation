using System;

using Autofac;

using FGS.Pump.Configuration.Abstractions;

namespace FGS.Pump.Configuration.DI
{
    public static class ComponentContextExtensions
    {
        public static string ResolveAppSetting(this IComponentContext context, string appSettingKey) =>
            context.Resolve<IAppSettings>()[appSettingKey];

        public static Lazy<string> ResolveAppSettingLazy(this IComponentContext context, string appSettingKey)
        {
            var lazyAppSettings = context.Resolve<Lazy<IAppSettings>>();
            return new Lazy<string>(() => lazyAppSettings.Value[appSettingKey]);
        }
    }
}
