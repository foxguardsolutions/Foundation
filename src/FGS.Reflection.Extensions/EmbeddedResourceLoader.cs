using System.IO;
using System.Reflection;

namespace FGS.Reflection.Extensions
{
    /// <summary>
    /// Provides functionality that helps assist with loading assembly-embedded resources.
    /// </summary>
    public static class EmbeddedResourceLoader
    {
        /// <summary>
        /// Gets the contents of a resource embedded in the current assembly.
        /// </summary>
        /// <param name="resourceNamespace">The namespace to retrieve the resource from.</param>
        /// <param name="name">The name of the resource.</param>
        /// <returns>The contents of the requested resource.</returns>
        public static string GetEmbeddedResource(string resourceNamespace, string name)
        {
            var assembly = Assembly.GetCallingAssembly();
            var resourceName = $"{resourceNamespace}.{name}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}
