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
using System.Reflection;
using System.Threading.Tasks;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Aggregates.Snapshot.Strategies;
using Akkatecture.Core;
using Akkatecture.Events;
using Akkatecture.Extensions;
using SnapshotMetadata = Akkatecture.Aggregates.Snapshot.SnapshotMetadata;

namespace Akkatecture.Sagas.AggregateSaga
{
    public abstract class AggregateSaga<TAggregateSaga, TIdentity, TSagaState> : ReceivePersistentActor, IAggregateSaga<TIdentity>
        where TAggregateSaga : AggregateSaga<TAggregateSaga, TIdentity, TSagaState>
        where TIdentity : SagaId<TIdentity>
        where TSagaState : SagaState<TAggregateSaga,TIdentity, IMessageApplier<TAggregateSaga, TIdentity>>
    {
        private static readonly IReadOnlyDictionary<Type, Action<TSagaState, IAggregateEvent>> ApplyMethodsFromState;
        private static readonly IReadOnlyDictionary<Type, Action<TSagaState, IAggregateSnapshot>> HydrateMethodsFromState;
        private static readonly IAggregateName SagaName = typeof(TAggregateSaga).GetSagaName();
        private readonly List<IEventApplier<TAggregateSaga, TIdentity>> _eventAppliers = new List<IEventApplier<TAggregateSaga, TIdentity>>();
        private readonly List<ISnapshotHydrater<TAggregateSaga, TIdentity>> _snapshotHydraters = new List<ISnapshotHydrater<TAggregateSaga, TIdentity>>();
        private readonly Dictionary<Type, Action<object>> _eventHandlers = new Dictionary<Type, Action<object>>();
        private readonly Dictionary<Type, Action<object>> _snapshotHandlers = new Dictionary<Type, Action<object>>();
        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(100);
        protected ILoggingAdapter Logger { get; }
        protected IEventDefinitionService _eventDefinitionService;
        protected ISnapshotDefinitionService _snapshotDefinitionService;
        protected ISnapshotStrategy SnapshotStrategy { get; set; } = SnapshotNeverStrategy.Instance;
        public int? SnapshotVersion { get; private set; }
        public TSagaState State { get; protected set; }
        public IAggregateName Name => SagaName;
        public override string PersistenceId { get; }
        public TIdentity Id { get; }
        public long Version { get; protected set; }
        public bool IsNew => Version <= 0;
        public override Recovery Recovery => new Recovery(SnapshotSelectionCriteria.Latest);
        public AggregateSagaSettings Settings { get; }

        static AggregateSaga()
        {
            ApplyMethodsFromState = typeof(TSagaState)
                .GetAggregateStateEventApplyMethods<TAggregateSaga, TIdentity, TSagaState>();

            HydrateMethodsFromState = typeof(TSagaState)
                .GetAggregateSnapshotHydrateMethods<TAggregateSaga, TIdentity, TSagaState>();
        }

        protected AggregateSaga()
        {
            Logger = Context.GetLogger();
            Settings = new AggregateSagaSettings(Context.System.Settings.Config);
            var idValue = Context.Self.Path.Name;
            PersistenceId = idValue;
            Id = (TIdentity) Activator.CreateInstance(typeof(TIdentity), idValue);

            if (Id == null)
            {
                throw new InvalidOperationException(
                    $"Identity for Saga '{Id.GetType().PrettyPrint()}' could not be activated.");
            }

            if ((this as TAggregateSaga) == null)
            {
                throw new InvalidOperationException(
                    $"AggregateSaga '{GetType().PrettyPrint()}' specifies '{typeof(TAggregateSaga).PrettyPrint()}' as generic argument, it should be its own type");
            }

            if (State == null)
            {
                try
                {
                    State = (TSagaState)Activator.CreateInstance(typeof(TSagaState));
                }
                catch
                {
                    Logger.Warning($"Unable to activate State for {GetType()}");
                }

            }

            if (Settings.AutoReceive)
            {
                InitReceives();
                InitAsyncReceives();
            }
            
            Register(State);

            if (Settings.UseDefaultEventRecover)
            {
                Recover<ICommittedEvent<TAggregateSaga, TIdentity, IAggregateEvent<TAggregateSaga, TIdentity>>>(Recover);
                Recover<RecoveryCompleted>(Recover);
            }
                

            if (Settings.UseDefaultSnapshotRecover)
                Recover<SnapshotOffer>(Recover);


            _eventDefinitionService = new EventDefinitionService(Logger);
            _snapshotDefinitionService = new SnapshotDefinitionService(Logger);

        }

        public void InitReceives()
        {
            var type = GetType();

            var subscriptionTypes =
                type
                    .GetSagaEventSubscriptionTypes();

            var methods = type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Handle")
                        return false;

                    var parameters = mi.GetParameters();

                    return
                        parameters.Length == 1;
                })
                .ToDictionary(
                    mi => mi.GetParameters()[0].ParameterType,
                    mi => mi);


            var method = type
                .GetBaseType("ReceivePersistentActor")
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Command") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1
                        && parameters[0].ParameterType.Name.Contains("Func");
                })
                .First();

            foreach (var subscriptionType in subscriptionTypes)
            {
                var funcType = typeof(Func<,>).MakeGenericType(subscriptionType, typeof(bool));
                var subscriptionFunction = Delegate.CreateDelegate(funcType, this, methods[subscriptionType]);
                var actorReceiveMethod = method.MakeGenericMethod(subscriptionType);

                actorReceiveMethod.Invoke(this, new[] { subscriptionFunction });
            }
        }

        public void InitAsyncReceives()
        {
            var type = GetType();

            var subscriptionTypes =
                type
                    .GetAsyncSagaEventSubscriptionTypes();

            var methods = type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Handle")
                        return false;

                    var parameters = mi.GetParameters();

                    return
                        parameters.Length == 1;
                })
                .ToDictionary(
                    mi => mi.GetParameters()[0].ParameterType,
                    mi => mi);


            var method = type
                .GetBaseType("ReceivePersistentActor")
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "CommandAsync") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 2
                        && parameters[0].ParameterType.Name.Contains("Func");
                })
                .First();

            foreach (var subscriptionType in subscriptionTypes)
            {
                var funcType = typeof(Func<,>).MakeGenericType(subscriptionType, typeof(Task));
                var subscriptionFunction = Delegate.CreateDelegate(funcType, this, methods[subscriptionType]);
                var actorReceiveMethod = method.MakeGenericMethod(subscriptionType);

                actorReceiveMethod.Invoke(this, new[] { subscriptionFunction, null });
            }
        }


        protected virtual void Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IMetadata metadata = null)
            where TAggregateEvent : IAggregateEvent<TAggregateSaga, TIdentity>
        {
            var committedEvent = From(aggregateEvent, Version, metadata);
            Persist(committedEvent, ApplyCommittedEvent);

        }

        public virtual void EmitAll<TAggregateEvent>(IEnumerable<TAggregateEvent> aggregateEvents, IMetadata metadata = null)
            where TAggregateEvent : IAggregateEvent<TAggregateSaga, TIdentity>
        {
            long version = Version;
            var comittedEvents = new List<CommittedEvent<TAggregateSaga, TIdentity, TAggregateEvent>>();
            foreach (var aggregateEvent in aggregateEvents)
            {
                var committedEvent = From(aggregateEvent, version + 1, metadata);
                comittedEvents.Add(committedEvent);
                version++;
            }

            PersistAll(comittedEvents, ApplyCommittedEvent);
        }

        public virtual CommittedEvent<TAggregateSaga, TIdentity, TAggregateEvent> From<TAggregateEvent>(TAggregateEvent aggregateEvent,
            long version, IMetadata metadata = null)
            where TAggregateEvent : IAggregateEvent<TAggregateSaga, TIdentity>
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }
            _eventDefinitionService.Load(typeof(TAggregateEvent));
            var eventDefinition = _eventDefinitionService.GetDefinition(typeof(TAggregateEvent));
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
                EventId = eventId,
                EventName = eventDefinition.Name,
                EventVersion = eventDefinition.Version
            };
            eventMetadata.Add(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }

            var committedEvent = new CommittedEvent<TAggregateSaga, TIdentity, TAggregateEvent>(Id, aggregateEvent, eventMetadata, now, Version);
            return committedEvent;
        }

        protected virtual IAggregateSnapshot<TAggregateSaga, TIdentity> CreateSnapshot()
        {
            Logger.Info($"[{Name}] With Id={Id} Attempted to Create a Snapshot, override the CreateSnapshot() method to return the snapshot data model.");
            return null;
        }

        protected void ApplyCommittedEvent<TAggregateEvent>(ICommittedEvent<TAggregateSaga, TIdentity, TAggregateEvent> committedEvent)
            where TAggregateEvent : IAggregateEvent<TAggregateSaga, TIdentity>
        {
            var applyMethods = GetEventApplyMethods(committedEvent.AggregateEvent);
            applyMethods(committedEvent.AggregateEvent);

            Logger.Info($"[{Name}] With Id={Id} Commited and Applied [{typeof(TAggregateEvent).PrettyPrint()}]");

            Version++;

            var domainEvent = new DomainEvent<TAggregateSaga, TIdentity, TAggregateEvent>(Id, committedEvent.AggregateEvent, committedEvent.Metadata, committedEvent.Timestamp, Version);

            Publish(domainEvent);

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
                        new ComittedSnapshot<TAggregateSaga, TIdentity, IAggregateSnapshot<TAggregateSaga, TIdentity>>(
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

        protected Action<IAggregateEvent> GetEventApplyMethods<TAggregateEvent>(TAggregateEvent aggregateEvent)
            where TAggregateEvent : IAggregateEvent<TAggregateSaga, TIdentity>
        {
            var eventType = aggregateEvent.GetType();

            Action<TSagaState, IAggregateEvent> applyMethod;
            if (!ApplyMethodsFromState.TryGetValue(eventType, out applyMethod))
            {
                throw new NotImplementedException(
                    $"Aggregate State '{State.GetType().PrettyPrint()}' does have an 'Apply' method that takes aggregate event '{eventType.PrettyPrint()}' as argument");
            }

            var aggregateApplyMethod = applyMethod.Bind(State);

            return aggregateApplyMethod;
        }
        
        protected void Register<TAggregateEvent>(Action<TAggregateEvent> handler)
            where TAggregateEvent : IAggregateEvent<TAggregateSaga, TIdentity>
        {
            var eventType = typeof(TAggregateEvent);
            if (_eventHandlers.ContainsKey(eventType))
            {
                throw new ArgumentException($"There's already a event handler registered for the aggregate event '{eventType.PrettyPrint()}'");
            }
            _eventHandlers[eventType] = e => handler((TAggregateEvent)e);
        }

        protected virtual void ApplyEvent(IAggregateEvent<TAggregateSaga, TIdentity> aggregateEvent)
        {
            var eventType = aggregateEvent.GetType();
            if (_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType](aggregateEvent);
            }
            else if (_eventAppliers.Any(ea => ea.Apply((TAggregateSaga)this, aggregateEvent)))
            {
                // Already done
            }

            var eventApplier = GetEventApplyMethods(aggregateEvent);

            eventApplier(aggregateEvent);

            Version++;
        }
        
        protected void Register(IEventApplier<TAggregateSaga, TIdentity> eventApplier)
        {
            _eventAppliers.Add(eventApplier);
        }

        public void ApplyEvents(IEnumerable<IAggregateEvent> aggregateEvents)
        {
            if (Version > 0)
            {
                throw new InvalidOperationException($"Aggregate '{GetType().PrettyPrint()}' with ID '{Id}' already has events");
            }

            foreach (var aggregateEvent in aggregateEvents)
            {
                var e = aggregateEvent as IAggregateEvent<TAggregateSaga, TIdentity>;
                if (e == null)
                {
                    throw new ArgumentException($"Aggregate event of type '{aggregateEvent.GetType()}' does not belong with aggregate '{this}',");
                }

                ApplyEvent(e);
            }
        }

        public void ApplyEvents(IReadOnlyCollection<IDomainEvent> domainEvents)
        {
            if (!domainEvents.Any())
            {
                return;
            }

            ApplyEvents(domainEvents.Select(e => e.GetAggregateEvent()));
            foreach (var domainEvent in domainEvents.Where(e => e.Metadata.ContainsKey(MetadataKeys.SourceId)))
            {
                _previousSourceIds.Put(domainEvent.Metadata.SourceId);
            }
            Version = domainEvents.Max(e => e.AggregateSequenceNumber);
        }

        protected virtual bool Recover(IAggregateEvent<TAggregateSaga, TIdentity> aggregateEvent)
        {
            try
            {
                Logger.Debug($"Recovering with event of type [{aggregateEvent.GetType().PrettyPrint()}] ");
                ApplyEvent(aggregateEvent);
            }
            catch (Exception exception)
            {
                Logger.Error($"Recovering with event of type [{aggregateEvent.GetType().PrettyPrint()}] caused an exception {exception.GetType().PrettyPrint()}");
                return false;
            }

            return true;
        }

        protected virtual bool Recover(ICommittedEvent<TAggregateSaga, TIdentity, IAggregateEvent<TAggregateSaga, TIdentity>> domainEvent)
        {
            try
            {
                Logger.Debug($"Recovering with event of type [{domainEvent.GetType().PrettyPrint()}] ");
                ApplyEvent(domainEvent.AggregateEvent);
            }
            catch (Exception exception)
            {
                Logger.Error($"Recovering with event of type [{domainEvent.GetType().PrettyPrint()}] caused an exception {exception.GetType().PrettyPrint()}");
                return false;
            }

            return true;
        }

        protected virtual bool Recover(SnapshotOffer aggregateSnapshotOffer)
        {
            Logger.Info($"Aggregate [{Name}] With Id={Id} has received a SnapshotOffer of type {aggregateSnapshotOffer.Snapshot.GetType().PrettyPrint()}");
            try
            {
                var comittedSnapshot = aggregateSnapshotOffer.Snapshot as ComittedSnapshot<TAggregateSaga, TIdentity, IAggregateSnapshot<TAggregateSaga, TIdentity>>;
                HydrateSnapshot(comittedSnapshot.AggregateSnapshot, aggregateSnapshotOffer.Metadata.SequenceNr);
            }
            catch (Exception exception)
            {
                Logger.Error($"Recovering with snapshot of type [{aggregateSnapshotOffer.Snapshot.GetType().PrettyPrint()}] caused an exception {exception.GetType().PrettyPrint()}");

                return false;
            }

            return true;
        }

        protected virtual void HydrateSnapshot(IAggregateSnapshot<TAggregateSaga, TIdentity> aggregateSnapshot, long version)
        {
            var snapshotType = aggregateSnapshot.GetType();
            if (_snapshotHandlers.ContainsKey(snapshotType))
            {
                _snapshotHandlers[snapshotType](aggregateSnapshot);
            }
            else if (_snapshotHydraters.Any(ea => ea.Hydrate((TAggregateSaga)this, aggregateSnapshot)))
            {
                // Already done
            }

            var snapshotHydrater = GetSnapshotHydrateMethods(aggregateSnapshot);

            snapshotHydrater(aggregateSnapshot);

            Version = version;
        }

        protected Action<IAggregateSnapshot> GetSnapshotHydrateMethods<TAggregateSnapshot>(TAggregateSnapshot aggregateEvent)
            where TAggregateSnapshot : IAggregateSnapshot<TAggregateSaga, TIdentity>
        {
            var snapshotType = aggregateEvent.GetType();

            Action<TSagaState, IAggregateSnapshot> hydrateMethod;
            if (!HydrateMethodsFromState.TryGetValue(snapshotType, out hydrateMethod))
            {
                throw new NotImplementedException(
                    $"Aggregate State '{State.GetType().PrettyPrint()}' does have an 'Apply' method that takes aggregate event '{snapshotType.PrettyPrint()}' as argument");
            }

            var snapshotHydrateMethod = hydrateMethod.Bind(State);

            return snapshotHydrateMethod;
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
    }
}