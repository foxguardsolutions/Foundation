using System.Reflection;

namespace FGS.Pump.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the <see cref="AssemblyInformationalVersionAttribute"/>of the assembly if it exists, or,
        /// if it doesn't exist, the version of the assembly.
        /// </summary>
        public static string GetDisplayVersion(this Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            return attribute?.InformationalVersion ?? assembly.GetName().Version.ToString();
        }
    }
}