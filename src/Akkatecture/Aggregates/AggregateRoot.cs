using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates
{
    //TODO Uncomitted Events
    public abstract class AggregateRoot<TAggregate, TIdentity, TState> : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TAggregate : AggregateRoot<TAggregate, TIdentity, TState>
        where TState : AggregateState<TAggregate,TIdentity, IEventApplier<TAggregate,TIdentity>>
        where TIdentity : IIdentity
    {
        private static readonly IReadOnlyDictionary<Type, Action<TAggregate, IAggregateEvent>> ApplyMethods;
        private static readonly IAggregateName AggregateName = typeof(TAggregate).GetAggregateName();
        public TState State { get; protected set; } = null;
        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(10);
        private ILoggingAdapter Logger { get; set; }

        public IAggregateName Name => AggregateName;
        public override string PersistenceId => Id.Value;
        public TIdentity Id { get; }
        public int Version { get; protected set; }
        public bool IsNew => Version <= 0;
        
        static AggregateRoot()
        {
            ApplyMethods = typeof(TAggregate).GetAggregateEventApplyMethods<TAggregate, TIdentity, TAggregate>();
        }

        protected AggregateRoot(TIdentity id)
        {
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
                    State = (TState)Activator.CreateInstance(typeof(TState));
                }
                catch
                {
                    Logger.Warning($"Unable to activate State for {GetType()}");
                }
                
            }

            Id = id;
            Logger = Context.GetLogger();
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
            var now = DateTimeOffset.Now;
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
            
            var type = typeof(TAggregateEvent);
            var applyMethod = ApplyMethods[type];
            var aggregateApplyMethod = applyMethod.Bind(this as TAggregate);
            
            //Need to figure out how to Persist CommittedEvent with Metadata and then publish the DomainEvent (not aggregate event to event stream)

            Persist(aggregateEvent, aggregateApplyMethod);

            Logger.Info($"[{Name}] With Id={Id} Commited [{typeof(TAggregateEvent)}]");


            //Eventsourced.LastSequenceNr or Version
            var domainEvent = new DomainEvent<TAggregate,TIdentity,TAggregateEvent>(aggregateEvent,eventMetadata,now,Id,Version);

            Publish(domainEvent);
        }

        protected virtual void Publish<TEvent>(TEvent aggregateEvent)
        {
            Context.System.EventStream.Publish(aggregateEvent);
            Logger.Info($"[{Name}] With Id={Id} Published [{typeof(TEvent)}]");
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
            else
            {
                Action<TAggregate, IAggregateEvent> applyMethod;
                if (!ApplyMethods.TryGetValue(eventType, out applyMethod))
                {
                    throw new NotImplementedException(
                        $"Aggregate '{Name}' does have an 'Apply' method that takes aggregate event '{eventType.PrettyPrint()}' as argument");
                }

                applyMethod(this as TAggregate, aggregateEvent);
            }

            Version++;
        }

        protected virtual bool Recover(IAggregateEvent<TAggregate, TIdentity> aggregateEvent)
        {
            try
            {
                ApplyEvent(aggregateEvent);
            }
            catch
            {
                return false;
            }

            return true;
        }

        protected virtual bool Recover(SnapshotOffer aggregateSnapshotOffer)
        {
            try
            {
                State = aggregateSnapshotOffer.Snapshot as TState;
            }
            catch
            {
                return false;
            }

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
