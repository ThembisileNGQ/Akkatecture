using System;
using System.Collections.Generic;
using System.Text;
using Akka.Event;
using Akkatecture.Aggregates;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Events
{
    public class EventDefinitionService : VersionedTypeDefinitionService<IAggregateEvent, EventVersionAttribute, EventDefinition>, IEventDefinitionService
    {
        public EventDefinitionService(ILoggingAdapter logger)
            : base(logger)
        {
        }

        protected override EventDefinition CreateDefinition(int version, Type type, string name)
        {
            return new EventDefinition(version, type, name);
        }
    }
}
