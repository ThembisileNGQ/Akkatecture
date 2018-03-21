using System;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates
{
    public class DomainEvent<TAggregate, TIdentity, TAggregateEvent> : IDomainEvent<TAggregate, TIdentity, TAggregateEvent>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
    {
        public Type AggregateType => typeof(TAggregate);
        public Type IdentityType => typeof(TIdentity);
        public Type EventType => typeof(TAggregateEvent);

        public int AggregateSequenceNumber { get; }
        public TAggregateEvent AggregateEvent { get; }
        public TIdentity AggregateIdentity { get; }
        public IMetadata Metadata { get; }
        public DateTimeOffset Timestamp { get; }

        public DomainEvent(
            TAggregateEvent aggregateEvent,
            IMetadata metadata,
            DateTimeOffset timestamp,
            TIdentity aggregateIdentity,
            int aggregateSequenceNumber)
        {
            if (aggregateEvent == null) throw new ArgumentNullException(nameof(aggregateEvent));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            if (timestamp == default(DateTimeOffset)) throw new ArgumentNullException(nameof(timestamp));
            if (aggregateIdentity == null || string.IsNullOrEmpty(aggregateIdentity.Value)) throw new ArgumentNullException(nameof(aggregateIdentity));
            if (aggregateSequenceNumber <= 0) throw new ArgumentOutOfRangeException(nameof(aggregateSequenceNumber));

            AggregateEvent = aggregateEvent;
            Metadata = metadata;
            Timestamp = timestamp;
            AggregateIdentity = aggregateIdentity;
            AggregateSequenceNumber = aggregateSequenceNumber;
        }

        public IIdentity GetIdentity()
        {
            return AggregateIdentity;
        }

        public IAggregateEvent GetAggregateEvent()
        {
            return AggregateEvent;
        }

        public override string ToString()
        {
            return $"{AggregateType.PrettyPrint()} v{AggregateSequenceNumber}/{EventType.PrettyPrint()}:{AggregateIdentity}";
        }
    }
}