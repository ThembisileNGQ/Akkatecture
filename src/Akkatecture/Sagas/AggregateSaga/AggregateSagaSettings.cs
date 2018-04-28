using Akka.Configuration;
using Akkatecture.Configuration;

namespace Akkatecture.Sagas.AggregateSaga
{
    public class AggregateSagaSettings
    {
        public readonly bool AutoReceive;
        public readonly bool UseDefaultEventRecover;
        public readonly bool UseDefaultSnapshotRecover;
        public AggregateSagaSettings(Config config)
        {
            var aggregateSagaConfig = config.WithFallback(AkkatectureDefaultSettings.DefaultConfig());
            aggregateSagaConfig = aggregateSagaConfig.GetConfig("akkatecture.aggregate-saga");

            AutoReceive = aggregateSagaConfig.GetBoolean("auto-receive");
            UseDefaultEventRecover = aggregateSagaConfig.GetBoolean("use-default-event-recover");
            UseDefaultSnapshotRecover = aggregateSagaConfig.GetBoolean("use-default-snapshot-recover");
        }
    }
}