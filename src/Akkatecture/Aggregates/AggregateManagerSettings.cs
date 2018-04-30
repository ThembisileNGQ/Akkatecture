using Akka.Configuration;
using Akkatecture.Configuration;

namespace Akkatecture.Aggregates
{
    public class AggregateManagerSettings
    {
        public readonly bool HandleDeadLetters;
        public readonly bool AutoDispatchOnReceive;

        public AggregateManagerSettings(Config config)
        {
            var aggregateManagerConfig = config.WithFallback(AkkatectureDefaultSettings.DefaultConfig());
            aggregateManagerConfig = aggregateManagerConfig.GetConfig("akkatecture.aggregate-manager");

            HandleDeadLetters = aggregateManagerConfig.GetBoolean("handle-deadletters");
            AutoDispatchOnReceive = aggregateManagerConfig.GetBoolean("auto-dispatch-on-receive");
        }
    }
}