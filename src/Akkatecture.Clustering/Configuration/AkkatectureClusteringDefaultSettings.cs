using Akka.Configuration;

namespace Akkatecture.Clustering.Configuration
{
    public class AkkatectureClusteringDefaultSettings
    {
        public static Config DefaultConfig()
        {
            return ConfigurationFactory.FromResource<AkkatectureClusteringDefaultSettings>("Akkatecture.Clustering.Configuration.default.conf");
        }
    }
}