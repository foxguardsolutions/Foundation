using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace FGS.Extensions.Configuration.AWS.ElasticBeanstalk.IisEnv
{
    /// <summary>
    /// Represents a <see cref="IConfigurationSource"/> that retrieves data from the "IIS Environment" values in a "container configuration" file as
    /// part of an AWS Elastic Beanstalk instance.
    /// </summary>
    public class AwsElasticBeanstalkContainerConfigurationConfigurationSource : JsonConfigurationSource
    {
        /// <summary>
        /// Builds the <see cref="AwsElasticBeanstalkIisEnvConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="AwsElasticBeanstalkIisEnvConfigurationProvider"/></returns>
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new AwsElasticBeanstalkIisEnvConfigurationProvider(this);
        }
    }
}
