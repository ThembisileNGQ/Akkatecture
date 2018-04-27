using Akka.Configuration;
using Akkatecture.Configuration;

namespace Akkatecture.Subscribers
{
    public class DomainEventSubscriberSettings
    {
        public readonly bool AutoSubscribe;
        public readonly bool AutoReceive;

        public DomainEventSubscriberSettings(Config config)
        {
            var domainEventSubscriberConfig = config.WithFallback(AkkatectureDefaultSettings.DefaultConfig());
            domainEventSubscriberConfig = domainEventSubscriberConfig.GetConfig("akkatecture.domain-event-subscriber");

            AutoSubscribe = domainEventSubscriberConfig.GetBoolean("auto-subscribe");
            AutoReceive = domainEventSubscriberConfig.GetBoolean("auto-receive");
        }
    }
}