using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates
{
    public abstract class AggregateRoot<TAggregate, TIdentity, TAggregateState> : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TAggregate : AggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregateState : AggregateState<TAggregate,TIdentity, IEventApplier<TAggregate,TIdentity>>
        where TIdentity : IIdentity
    {
        private static readonly IReadOnlyDictionary<Type, Action<TAggregateState, IAggregateEvent>> ApplyMethodsFromState;
        private static readonly IAggregateName AggregateName = typeof(TAggregate).GetAggregateName();
        
        public TAggregateState State { get; protected set; }
        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(10);
        protected ILoggingAdapter Logger { get; set; }

        public IAggregateName Name => AggregateName;
        public override string PersistenceId { get; }
        public TIdentity Id { get; }
        public long Version { get; protected set; }
        public bool IsNew => Version <= 0;
        public AggregateRootSettings Settings { get; }
        static AggregateRoot()
        {
            ApplyMethodsFromState = typeof(TAggregateState)
                .GetAggregateStateEventApplyMethods<TAggregate, TIdentity, TAggregateState>();


        }

        protected AggregateRoot(TIdentity id)
        {
            Logger = Context.GetLogger();
            if (id == null) throw new ArgumentNullException(nameof(id));
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
                    Logger.Warning($"Unable to activate State for {GetType()}");
                }
                
            }
            
            Settings = new AggregateRootSettings(Context.System.Settings.Config);
            Id = id;
            PersistenceId = id.Value;
            Register(State);

            if (Settings.UseDefaultEventRecover)
            {
                Recover<DomainEvent<TAggregate, TIdentity, IAggregateEvent<TAggregate, TIdentity>>>(Recover);
                Recover<IAggregateEvent<TAggregate, TIdentity>>(Recover);
                Recover<RecoveryCompleted>(Recover);
            }
                

            if (Settings.UseDefaultSnapshotRecover)
                Recover<SnapshotOffer>(Recover);

        }
        
        
        protected void SetSourceIdHistory(int count)
        {
            _previousSourceIds = new CircularBuffer<ISourceId>(count);
        }

        public bool HasSourceId(ISourceId sourceId)
        {
            return !sourceId.IsNone() && _previousSourceIds.Any(s => s.Value == sourceId.Value);
        }

        

        protected virtual void Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IMetadata metadata = null)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }

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
                EventId = eventId
            };
            eventMetadata.Add(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }
            
            var aggregateApplyMethod = GetEventApplyMethods(aggregateEvent);

            Persist(aggregateEvent, aggregateApplyMethod);

            Logger.Info($"[{Name}] With Id={Id} Commited [{typeof(TAggregateEvent).PrettyPrint()}]");

            Version++;
                
            var domainEvent = new DomainEvent<TAggregate,TIdentity,TAggregateEvent>(aggregateEvent,eventMetadata,now,Id,Version);

            Publish(domainEvent);
        }
        
        protected virtual void Signal<TAggregateEvent>(TAggregateEvent aggregateEvent, IMetadata metadata = null)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }

            var aggregateSequenceNumber = Version;
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
            };

            eventMetadata.Add(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }

            Logger.Info($"[{Name}] With Id={Id} Commited [{typeof(TAggregateEvent).PrettyPrint()}]");

            var domainEvent = new DomainEvent<TAggregate,TIdentity,TAggregateEvent>(aggregateEvent,eventMetadata,now,Id,Version);

            Publish(domainEvent);
        }

        protected virtual void Throw<TAggregateEvent>(TAggregateEvent aggregateEvent, IMetadata metadata = null)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            Signal(aggregateEvent,metadata);
        }

        protected virtual void Publish<TEvent>(TEvent aggregateEvent)
        {
            Context.System.EventStream.Publish(aggregateEvent);
            Logger.Info($"[{Name}] With Id={Id} Published [{typeof(TEvent).PrettyPrint()}]");
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

        public IIdentity GetIdentity()
        {
            return Id;
        }

        public void ApplyEvents(IEnumerable<IAggregateEvent> aggregateEvents)
        {
            if (Version > 0)
            {
                throw new InvalidOperationException($"Aggregate '{GetType().PrettyPrint()}' with ID '{Id}' already has events");
            }

            foreach (var aggregateEvent in aggregateEvents)
            {
                var e = aggregateEvent as IAggregateEvent<TAggregate, TIdentity>;
                if (e == null)
                {
                    throw new ArgumentException($"Aggregate event of type '{aggregateEvent.GetType()}' does not belong with aggregate '{this}',");
                }

                ApplyEvent(e);
            }
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
            {
                throw new NotImplementedException(
                    $"Aggregate State '{State.GetType().PrettyPrint()}' does have an 'Apply' method that takes aggregate event '{eventType.PrettyPrint()}' as argument");
            }

            var aggregateApplyMethod = applyMethod.Bind(State);

            return aggregateApplyMethod;
        }

        protected Action<IAggregateEvent> GetDomainEventApplyMethods<TDomainEvent, TAggregateEvent>(TDomainEvent domainEvent)
            where TDomainEvent : IDomainEvent<TAggregate,TIdentity,TAggregateEvent>
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            return GetEventApplyMethods(domainEvent.AggregateEvent);
        }

        protected virtual void ApplyEvent(IAggregateEvent<TAggregate, TIdentity> aggregateEvent)
        {
            var eventType = aggregateEvent.GetType();
            if (_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType](aggregateEvent);
            }
            else if (_eventAppliers.Any(ea => ea.Apply((TAggregate)this, aggregateEvent)))
            {
                // Already done
            }
           
            var eventApplier = GetEventApplyMethods(aggregateEvent);

            eventApplier(aggregateEvent);

            Version++;
        }

        protected virtual bool Recover(IAggregateEvent<TAggregate, TIdentity> aggregateEvent)
        {
            try
            {
                //TODO event upcasting goes here
                Logger.Debug($"Recovering with event of type [{aggregateEvent.GetType().PrettyPrint()}] ");
                ApplyEvent(aggregateEvent);
            }
            catch(Exception exception)
            {
                Logger.Error($"Recovering with event of type [{aggregateEvent.GetType().PrettyPrint()}] caused an exception {exception.GetType().PrettyPrint()}");
                return false;
            }

            return true;
        }

        protected virtual bool Recover(IDomainEvent<TAggregate, TIdentity, IAggregateEvent<TAggregate,TIdentity>> domainEvent)
        {
            try
            {

                //TODO event upcasting goes here
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
            try
            {
                State = aggregateSnapshotOffer.Snapshot as TAggregateState;
                Version = LastSequenceNr;

            }
            catch (Exception exception)
            {
                Logger.Error($"Recovering with snapshot of type [{aggregateSnapshotOffer.Snapshot.GetType().PrettyPrint()}] caused an exception {exception.GetType().PrettyPrint()}");

                return false;
            }

            return true;
        }

        protected virtual bool Recover(RecoveryCompleted recoveryCompleted)
        {
            
            return true;
        }

        private readonly Dictionary<Type, Action<object>> _eventHandlers = new Dictionary<Type, Action<object>>();
        protected void Register<TAggregateEvent>(Action<TAggregateEvent> handler)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            var eventType = typeof(TAggregateEvent);
            if (_eventHandlers.ContainsKey(eventType))
            {
                throw new ArgumentException($"There's already a event handler registered for the aggregate event '{eventType.PrettyPrint()}'");
            }
            _eventHandlers[eventType] = e => handler((TAggregateEvent)e);
        }

        private readonly List<IEventApplier<TAggregate, TIdentity>> _eventAppliers = new List<IEventApplier<TAggregate, TIdentity>>();

        protected void Register(IEventApplier<TAggregate, TIdentity> eventApplier)
        {
            _eventAppliers.Add(eventApplier);
        }

        public override string ToString()
        {
            return $"{GetType().PrettyPrint()} v{Version}";
        }
        
    }
}
