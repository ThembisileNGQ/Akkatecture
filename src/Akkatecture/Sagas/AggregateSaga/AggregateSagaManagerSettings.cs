using Akka.Configuration;
using Akkatecture.Configuration;

namespace Akkatecture.Sagas.AggregateSaga
{
    public class AggregateSagaManagerSettings
    {
        public readonly bool AutoSubscribe;
        public readonly bool AutoSpawnOnReceive;
        public AggregateSagaManagerSettings(Config config)
        {
            var aggregateSagaManagerConfig = config.WithFallback(AkkatectureDefaultSettings.DefaultConfig());
            aggregateSagaManagerConfig = aggregateSagaManagerConfig.GetConfig("akkatecture.aggregate-saga-manager");

            AutoSpawnOnReceive = aggregateSagaManagerConfig.GetBoolean("auto-spawn-on-receive");
            AutoSubscribe = aggregateSagaManagerConfig.GetBoolean("auto-subscribe");
        }
    }
}