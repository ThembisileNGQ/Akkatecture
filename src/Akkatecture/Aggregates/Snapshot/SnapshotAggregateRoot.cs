// The MIT License (MIT)
//
// Copyright (c) 2018 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Akka.Persistence;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates.Snapshot
{
    public abstract class SnapshotAggregateRoot<TAggregate, TIdentity, TAggregateState> : SnapshotAggregateRoot<
        TAggregate, TIdentity, TAggregateState, IAggregateSnapshot<TAggregate, TIdentity>>
        where TAggregate : SnapshotAggregateRoot<TAggregate, TIdentity, TAggregateState, IAggregateSnapshot<TAggregate, TIdentity>>
        where TAggregateState : SnapshotAggregateState<TAggregate, TIdentity, ISnapshotHydrater<TAggregate,TIdentity>>
        where TIdentity : IIdentity
    {
        protected SnapshotAggregateRoot(
            TIdentity id, 
            ISnapshotStrategy snapshotStrategy) 
            : base(id, snapshotStrategy)
        {
        }
    }
    public abstract class SnapshotAggregateRoot<TAggregate, TIdentity, TAggregateState, TSnapshot> : AggregateRoot<TAggregate, TIdentity, TAggregateState>, ISnapshotAggregateRoot<TIdentity, TSnapshot>
        where TAggregate : SnapshotAggregateRoot<TAggregate, TIdentity, TAggregateState, TSnapshot>
        where TAggregateState : SnapshotAggregateState<TAggregate, TIdentity, ISnapshotHydrater<TAggregate,TIdentity>>
        where TIdentity : IIdentity
        where TSnapshot : IAggregateSnapshot<TAggregate, TIdentity>
    {
        protected ISnapshotStrategy SnapshotStrategy { get; }
        public int? SnapshotVersion { get; private set; }

        protected SnapshotAggregateRoot(
            TIdentity id,
            ISnapshotStrategy snapshotStrategy)
            : base(id)
        {
            SnapshotStrategy = snapshotStrategy;

            if (Settings.UseDefaultSnapshotRecover)
                Recover<SnapshotOffer>(Recover);
        }

        protected virtual bool Recover(SnapshotOffer aggregateSnapshotOffer)
        {
            try
            {
                var snapshot = aggregateSnapshotOffer.Snapshot as IAggregateSnapshot<TAggregate,TIdentity>;
                Version = aggregateSnapshotOffer.Metadata.SequenceNr;
                State.Hydrate(this as TAggregate, snapshot);
            }
            catch (Exception exception)
            {
                Logger.Error($"Recovering with snapshot of type [{aggregateSnapshotOffer.Snapshot.GetType().PrettyPrint()}] caused an exception {exception.GetType().PrettyPrint()}");

                return false;
            }

            return true;
        }


        public override void Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IMetadata metadata = null)
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }
            _eventDefinitionService.Load(typeof(TAggregateEvent));
            var eventDefinition = _eventDefinitionService.GetDefinition(typeof(TAggregateEvent));
            var aggregateSequenceNumber = Version + 1;
            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{Id.Value}-v{aggregateSequenceNumber}");
            var now = DateTimeOffset.UtcNow;
            var eventMetadata = new Metadata
            {
                Timestamp = now,
                AggregateSequenceNumber = aggregateSequenceNumber,
                AggregateName = Name.Value,
                AggregateId = Id.Value,
                EventId = eventId,
                EventName = eventDefinition.Name,
                EventVersion = eventDefinition.Version
            };
            eventMetadata.Add(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }

            var committedEvent = new CommittedEvent<TAggregate, TIdentity, TAggregateEvent>(Id, aggregateEvent, eventMetadata, now, Version);
            Persist(committedEvent, ApplyCommittedEvents);

            Logger.Info($"[{Name}] With Id={Id} Commited [{typeof(TAggregateEvent).PrettyPrint()}]");

            Version++;

            var domainEvent = new DomainEvent<TAggregate, TIdentity, TAggregateEvent>(Id, aggregateEvent, eventMetadata, now, Version);

            Publish(domainEvent);

            if (SnapshotStrategy.ShouldCreateSnapshot(this))
            {
                var aggregateSnapshot = CreateSnapshot();
                
                SaveSnapshot(aggregateSnapshot);
            }
        }

        protected abstract TSnapshot CreateSnapshot();
    }
}