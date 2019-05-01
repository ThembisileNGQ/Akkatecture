// The MIT License (MIT)
//
// Copyright (c) 2015-2019 Rasmus Mikkelsen
// Copyright (c) 2015-2019 eBay Software Foundation
// Modified from original source https://github.com/eventflow/EventFlow
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
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
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Aggregates.ExecutionResults;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Aggregates.Snapshot.Strategies;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Events;
using Akkatecture.Extensions;
using SnapshotMetadata = Akkatecture.Aggregates.Snapshot.SnapshotMetadata;

namespace Akkatecture.Aggregates
{
    public abstract class AggregateRoot<TAggregate, TIdentity, TAggregateState> : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TAggregate : AggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregateState : AggregateState<TAggregate,TIdentity, IMessageApplier<TAggregate,TIdentity>>
        where TIdentity : IIdentity
    {
        private static readonly IReadOnlyDictionary<Type, Action<TAggregateState, IAggregateEvent>> ApplyMethodsFromState = typeof(TAggregateState).GetAggregateStateEventApplyMethods<TAggregate, TIdentity, TAggregateState>();
        private static readonly IReadOnlyDictionary<Type, Action<TAggregateState, IAggregateSnapshot>> HydrateMethodsFromState =  typeof(TAggregateState).GetAggregateSnapshotHydrateMethods<TAggregate, TIdentity, TAggregateState>();
        public static readonly IAggregateName AggregateName = typeof(TAggregate).GetAggregateName();
        protected CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(100);
        protected ICommand<TAggregate, TIdentity> PinnedCommand { get; private set; }
        protected object PinnedReply { get; private set; }
        
        protected ILoggingAdapter Logger { get; }
        protected IEventDefinitionService _eventDefinitionService;
        protected ISnapshotDefinitionService _snapshotDefinitionService;
        protected ISnapshotStrategy SnapshotStrategy { get; set; } = SnapshotNeverStrategy.Instance;
        public int? SnapshotVersion { get; private set; }
        public TAggregateState State { get;  }
        public IAggregateName Name => AggregateName;
        public override string PersistenceId { get; }
        public TIdentity Id { get; }
        public long Version { get; protected set; }
        public bool IsNew => Version <= 0;
        public override Recovery Recovery => new Recovery(SnapshotSelectionCriteria.Latest);
        public AggregateRootSettings Settings { get; }

        protected AggregateRoot(TIdentity id)
        {
            Logger = Context.GetLogger();
            Settings = new AggregateRootSettings(Context.System.Settings.Config);
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if ((this as TAggregate) == null)
            {
                throw new InvalidOperationException(
                    $"Aggregate '{GetType().PrettyPrint()}' specifies '{typeof(TAggregate).PrettyPrint()}' as generic argument, it should be its own type");
            }

            if (State == null)
            {
                try
                {
                    State = (TAggregateState)Activator.CreateInstance(typeof(TAggregateState));
                }
                catch
                {
                    Logger.Error($"Unable to activate State for {GetType()}");
                }

            }

            PinnedCommand = null;
            _eventDefinitionService = new EventDefinitionService(Logger);
            _snapshotDefinitionService = new SnapshotDefinitionService(Logger);
            Id = id;
            PersistenceId = id.Value;
            SetSourceIdHistory(100);
            if (Settings.UseDefaultSnapshotRecover)
            {
                Recover<SnapshotOffer>(Recover);
            }


            Command<SaveSnapshotSuccess>(SnapshotStatus);
            Command<SaveSnapshotFailure>(SnapshotStatus);

            if (Settings.UseDefaultEventRecover)
            {
                Recover<ICommittedEvent<TAggregate, TIdentity, IAggregateEvent<TAggregate, TIdentity>>>(Recover);
                Recover<RecoveryCompleted>(Recover);
            }

        }


        protected void SetSourceIdHistory(int count)
        {
            _previousSourceIds = new CircularBuffer<ISourceId>(count);
        }

        public bool HasSourceId(ISourceId sourceId)
        {
            return !sourceId.IsNone() && _previousSourceIds.Any(s => s.Value == sourceId.Value);
        }

        public IIdentity GetIdentity()
        {
            return Id;
        }

        public virtual void Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IMetadata metadata = null)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            var committedEvent = From(aggregateEvent, Version, metadata);
            Persist(committedEvent, ApplyCommittedEvent);
        }

        public virtual void EmitAll<TAggregateEvent>(IEnumerable<TAggregateEvent> aggregateEvents, IMetadata metadata = null)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            long version = Version;
            var comittedEvents = new List<CommittedEvent<TAggregate, TIdentity, TAggregateEvent>>();
            foreach (var aggregateEvent in aggregateEvents)
            {
                var committedEvent = From(aggregateEvent, version + 1, metadata);
                comittedEvents.Add(committedEvent);
                version++;
            }

            PersistAll(comittedEvents, ApplyCommittedEvent);
        }

        public virtual CommittedEvent<TAggregate, TIdentity, TAggregateEvent> From<TAggregateEvent>(TAggregateEvent aggregateEvent,
            long version, IMetadata metadata = null)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }
            _eventDefinitionService.Load(aggregateEvent.GetType());
            var eventDefinition = _eventDefinitionService.GetDefinition(aggregateEvent.GetType());
            var aggregateSequenceNumber = version + 1;
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
                SourceId = PinnedCommand.SourceId,
                EventId = eventId,
                EventName = eventDefinition.Name,
                EventVersion = eventDefinition.Version
            };
            eventMetadata.Add(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }
            
            var committedEvent = new CommittedEvent<TAggregate, TIdentity, TAggregateEvent>(Id, aggregateEvent,eventMetadata,now,aggregateSequenceNumber);
            return committedEvent;
        }
        protected virtual IAggregateSnapshot<TAggregate, TIdentity> CreateSnapshot()
        {
            Logger.Info($"[{Name}] With Id={Id} Attempted to Create a Snapshot, override the CreateSnapshot() method to return the snapshot data model.");
            return null;
        }

        protected void  ApplyCommittedEvent<TAggregateEvent>(ICommittedEvent<TAggregate, TIdentity, TAggregateEvent> committedEvent)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            var applyMethods = GetEventApplyMethods(committedEvent.AggregateEvent);
            applyMethods(committedEvent.AggregateEvent);

            Logger.Info($"[{Name}] With Id={Id} Commited and Applied [{typeof(TAggregateEvent).PrettyPrint()}]");

            Version++;

            var domainEvent = new DomainEvent<TAggregate,TIdentity,TAggregateEvent>(Id, committedEvent.AggregateEvent,committedEvent.Metadata,committedEvent.Timestamp,Version);

            Publish(domainEvent);
            ReplyIfAvailable();
            
            if (SnapshotStrategy.ShouldCreateSnapshot(this))
            {
                var aggregateSnapshot = CreateSnapshot();
                if (aggregateSnapshot != null)
                {
                    _snapshotDefinitionService.Load(aggregateSnapshot.GetType());
                    var snapshotDefinition = _snapshotDefinitionService.GetDefinition(aggregateSnapshot.GetType());
                    var snapshotMetadata = new SnapshotMetadata
                    {
                        AggregateId = Id.Value,
                        AggregateName = Name.Value,
                        AggregateSequenceNumber = Version,
                        SnapshotName = snapshotDefinition.Name,
                        SnapshotVersion = snapshotDefinition.Version
                    };

                    var commitedSnapshot =
                        new ComittedSnapshot<TAggregate, TIdentity, IAggregateSnapshot<TAggregate, TIdentity>>(
                            Id,
                            aggregateSnapshot,
                            snapshotMetadata,
                            committedEvent.Timestamp, Version);

                    SaveSnapshot(commitedSnapshot);
                }
            }

        }

        protected virtual void Publish<TEvent>(TEvent aggregateEvent)
        {
            Context.System.EventStream.Publish(aggregateEvent);
            Logger.Info($"[{Name}] With Id={Id} Published [{typeof(TEvent).PrettyPrint()}]");
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            if (message is Command<TAggregate, TIdentity> command)
            {
                PinnedCommand = command;
            }

            return base.AroundReceive(receive, message);
        }

        protected virtual void Reply(object replyMessage)
        {
            if(Sender == ActorRefs.NoSender || Sender == ActorRefs.Nobody)
            {
                // do nothing
            } else
            {
                PinnedReply = replyMessage;
            }
        }

        protected virtual void ReplyIfAvailable()
        {
            if(PinnedReply != null)
                Sender.Tell(PinnedReply);

            PinnedReply = null;
            PinnedCommand = null;
        }

        protected override void Unhandled(object message)
        {
            Logger.Info($"Aggregate with Id '{Id?.Value} has received an unhandled message {message.GetType().PrettyPrint()}'");
            base.Unhandled(message);
        }

        protected Action<IAggregateEvent> GetEventApplyMethods<TAggregateEvent>(TAggregateEvent aggregateEvent)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            var eventType = aggregateEvent.GetType();

            Action<TAggregateState, IAggregateEvent> applyMethod;
            if (!ApplyMethodsFromState.TryGetValue(eventType, out applyMethod))
                throw new NotImplementedException($"Aggregate State '{State.GetType().PrettyPrint()}' does have an 'Apply' method that takes aggregate event '{eventType.PrettyPrint()}' as argument");

            var aggregateApplyMethod = applyMethod.Bind(State);

            return aggregateApplyMethod;
        }

        protected Action<IAggregateSnapshot> GetSnapshotHydrateMethods<TAggregateSnapshot>(TAggregateSnapshot aggregateEvent)
            where TAggregateSnapshot : IAggregateSnapshot<TAggregate, TIdentity>
        {
            var snapshotType = aggregateEvent.GetType();

            Action<TAggregateState, IAggregateSnapshot> hydrateMethod;
            if (!HydrateMethodsFromState.TryGetValue(snapshotType, out hydrateMethod))
                throw new NotImplementedException($"Aggregate State '{State.GetType().PrettyPrint()}' does have an 'Apply' method that takes aggregate event '{snapshotType.PrettyPrint()}' as argument");
            


            var snapshotHydrateMethod = hydrateMethod.Bind(State);

            return snapshotHydrateMethod;
        }

        protected virtual void ApplyEvent(IAggregateEvent<TAggregate, TIdentity> aggregateEvent)
        {
            var eventApplier = GetEventApplyMethods(aggregateEvent);

            eventApplier(aggregateEvent);

            Version++;
        }

        protected virtual void HydrateSnapshot(IAggregateSnapshot<TAggregate, TIdentity> aggregateSnapshot, long version)
        {
            var snapshotHydrater = GetSnapshotHydrateMethods(aggregateSnapshot);

            snapshotHydrater(aggregateSnapshot);

            Version = version;
        }

        protected virtual bool Recover(ICommittedEvent<TAggregate, TIdentity, IAggregateEvent<TAggregate, TIdentity>> committedEvent)
        {
            try
            {
                Logger.Debug($"Recovering with event of type [{committedEvent.GetType().PrettyPrint()}] ");
                ApplyEvent(committedEvent.AggregateEvent);
            }
            catch(Exception exception)
            {
                Logger.Error($"Recovering with event of type [{committedEvent.GetType().PrettyPrint()}] caused an exception {exception.GetType().PrettyPrint()}");
                return false;
            }

            return true;
        }

        protected virtual bool Recover(SnapshotOffer aggregateSnapshotOffer)
        {
            Logger.Info($"Aggregate [{Name}] With Id={Id} has received a SnapshotOffer of type {aggregateSnapshotOffer.Snapshot.GetType().PrettyPrint()}");
            try
            {
                var comittedSnapshot = aggregateSnapshotOffer.Snapshot as ComittedSnapshot<TAggregate,TIdentity, IAggregateSnapshot<TAggregate, TIdentity>>;
                HydrateSnapshot(comittedSnapshot.AggregateSnapshot, aggregateSnapshotOffer.Metadata.SequenceNr);
            }
            catch (Exception exception)
            {
                Logger.Error($"Recovering with snapshot of type [{aggregateSnapshotOffer.Snapshot.GetType().PrettyPrint()}] caused an exception {exception.GetType().PrettyPrint()}");

                return false;
            }

            return true;
        }

        protected virtual void SetSnapshotStrategy(ISnapshotStrategy snapshotStrategy)
        {
            if (snapshotStrategy != null)
            {
                SnapshotStrategy = snapshotStrategy;
            }
        }
        protected virtual bool SnapshotStatus(SaveSnapshotSuccess snapshotSuccess)
        {
            Logger.Info($"Aggregate [{Name}] With Id={Id} Saved Snapshot at Version {snapshotSuccess.Metadata.SequenceNr}");
            return true;
        }

        protected virtual bool SnapshotStatus(SaveSnapshotFailure snapshotFailure)
        {
            Logger.Error($"Aggregate [{Name}] With Id={Id} Failed to save snapshot at version {snapshotFailure.Metadata.SequenceNr} because of {snapshotFailure.Cause}");
            return true;
        }


        protected virtual bool Recover(RecoveryCompleted recoveryCompleted)
        {
            Logger.Info($"Aggregate [{Name}] With Id={Id} has completed recovering from it's journal(s)");
            return true;
        }

        public override string ToString()
        {
            return $"{GetType().PrettyPrint()} v{Version}";
        }

        protected void Command<TCommand, TCommandHandler>(Predicate<TCommand> shouldHandle = null)
            where TCommand : ICommand<TAggregate, TIdentity>
            where TCommandHandler : CommandHandler<TAggregate, TIdentity, TCommand>
        {
            try
            {
                var handler = (TCommandHandler) Activator.CreateInstance(typeof(TCommandHandler));
                Command<TCommand>(x => handler.HandleCommand(this as TAggregate, Context, x),shouldHandle);
            }
            catch
            {
                Logger.Error($"Unable to Activate CommandHandler {typeof(TCommandHandler).PrettyPrint()} for {typeof(TAggregate).PrettyPrint()}");
            }

        }

    }

}
