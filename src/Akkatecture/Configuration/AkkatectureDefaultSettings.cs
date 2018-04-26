using Akka.Configuration;

namespace Akkatecture.Configuration
{
    public class AkkatectureDefaultSettings
    {
        public static Config DefaultConfig()
        {
            return ConfigurationFactory.FromResource<AkkatectureDefaultSettings>("Akkatecture.Configuration.reference.conf");
        }
    }
}