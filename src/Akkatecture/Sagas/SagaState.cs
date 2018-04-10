using System;
using System.Collections.Generic;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;

namespace Akkatecture.Sagas
{
    public abstract class SagaState<TSaga, TIdentity, TEventApplier> : ISagaState<TIdentity>, IEventApplier<TSaga, TIdentity>
        where TEventApplier : class, IEventApplier<TSaga, TIdentity>
        where TSaga : IAggregateRoot<TIdentity>
        where TIdentity : ISagaId
    {
        private static readonly IReadOnlyDictionary<Type, Action<TEventApplier, IAggregateEvent>> ApplyMethods;
        public SagaStatus Status { get; private set; }
        public Dictionary<SagaStatus, DateTimeOffset> SagaTimes { get; } = new Dictionary<SagaStatus, DateTimeOffset>();

        static SagaState()
        {
            ApplyMethods = typeof(TEventApplier).GetAggregateEventApplyMethods<TSaga, TIdentity, TEventApplier>();
        }


        protected SagaState()
        {

            Status = SagaStatus.NotStarted;
            StopWatch();

            var me = this as TEventApplier;
            if (me == null)
            {
                throw new InvalidOperationException(
                    $"Event applier of type '{GetType().PrettyPrint()}' has a wrong generic argument '{typeof(TEventApplier).PrettyPrint()}'");
            }
        }

        public bool Apply(
            TSaga aggregate,
            IAggregateEvent<TSaga, TIdentity> aggregateEvent)
        {
            var aggregateEventType = aggregateEvent.GetType();
            Action<TEventApplier, IAggregateEvent> applier;

            if (!ApplyMethods.TryGetValue(aggregateEventType, out applier))
            {
                return false;
            }

            applier((TEventApplier)(object)this, aggregateEvent);
            return true;
        }

        public virtual void Start()
        {
            Status = SagaStatus.Running;
            StopWatch();
        }

        public virtual void Complete()
        {
            Status = SagaStatus.Completed;
            StopWatch();
        }

        public virtual void Fail()
        {
            Status = SagaStatus.Failed;
            StopWatch();
        }

        public virtual void Cancel()
        {
            Status = SagaStatus.Cancelled;
            StopWatch();
        }

        public virtual void PartiallySucceed()
        {
            Status = SagaStatus.PartiallySucceeded;
            StopWatch();
        }

        public void StopWatch()
        {
            if (!SagaTimes.ContainsKey(Status))
            {
                SagaTimes.Add(Status,DateTimeOffset.UtcNow);
            }
        }
    }
}