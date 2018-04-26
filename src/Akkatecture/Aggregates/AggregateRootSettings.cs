using System;
using System.Collections.Generic;
using System.Text;
using Akka.Configuration;
using Akkatecture.Configuration;

namespace Akkatecture.Aggregates
{
    public class AggregateRootSettings
    {
        public readonly bool UseDefaultEventRecover;
        public readonly bool UseDefaultSnapshotRecover;

        public AggregateRootSettings(Config config)
        {
            var aggregateRootConfig = config.WithFallback(AkkatectureDefaultSettings.DefaultConfig());
            aggregateRootConfig = aggregateRootConfig.GetConfig("akkatecture.aggregate-root");

            UseDefaultEventRecover = aggregateRootConfig.GetBoolean("use-default-event-recover");
            UseDefaultSnapshotRecover = aggregateRootConfig.GetBoolean("use-default-snapshot-recover");
        }
    }
}
