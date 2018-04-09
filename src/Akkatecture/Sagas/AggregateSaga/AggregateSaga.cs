using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Sagas.AggregateSaga
{
    public abstract class AggregateSaga<TAggregateSaga, TIdentity, TSagaState> : ReceivePersistentActor, IAggregateSaga<TIdentity>
        where TAggregateSaga : AggregateSaga<TAggregateSaga, TIdentity, TSagaState>
        where TIdentity : SagaId<TIdentity>
        where TSagaState : SagaState<TAggregateSaga,TIdentity, IEventApplier<TAggregateSaga, TIdentity>>
    {
        private static readonly IReadOnlyDictionary<Type, Action<TSagaState, IAggregateEvent>> ApplyMethodsFromState;
        private static readonly IAggregateName AggregateName = typeof(TAggregateSaga).GetSagaName();
        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(10);
        public override string PersistenceId { get; } = Context.Self.Path.Name;
        protected ILoggingAdapter Logger { get; set; }
        public TSagaState State { get; protected set; }
        public TIdentity Id { get; }
        public IAggregateName Name => AggregateName;
        public long Version { get; protected set; }
        public bool IsNew => Version <= 0;

        static AggregateSaga()
        {
            ApplyMethodsFromState = typeof(TAggregateSaga)
                .GetAggregateStateEventApplyMethods<TAggregateSaga, TIdentity, TSagaState>();
        }

        protected AggregateSaga()
        {
            Logger = Context.GetLogger();
            var idValue = Context.Self.Path.Name;
            PersistenceId = Context.Self.Path.Name;
            Id = (TIdentity) Activator.CreateInstance(typeof(TIdentity), idValue);

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
            
            Register(State);

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

        private readonly List<IEventApplier<TAggregateSaga, TIdentity>> _eventAppliers = new List<IEventApplier<TAggregateSaga, TIdentity>>();
        
    
        private readonly Dictionary<Type, Action<object>> _eventHandlers = new Dictionary<Type, Action<object>>();
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

                //TODO event upcasting goes here
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

        protected virtual bool Recover(SnapshotOffer aggregateSnapshotOffer)
        {
            try
            {
                State = aggregateSnapshotOffer.Snapshot as TSagaState;
            }
            catch (Exception exception)
            {
                Logger.Error($"Recovering with snapshot of type [{aggregateSnapshotOffer.Snapshot.GetType().PrettyPrint()}] caused an exception {exception.GetType().PrettyPrint()}");

                return false;
            }

            return true;
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
    }
}