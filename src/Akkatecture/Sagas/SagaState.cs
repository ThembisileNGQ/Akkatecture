using System;
using System.Collections.Generic;

namespace Akkatecture.Sagas
{
    public abstract class SagaState<TSaga, TIdentity> : ISagaState<TSaga, TIdentity>
        where TSaga : ISaga
        where TIdentity : ISagaId
    {
        public SagaStatus Status { get; private set; }
        public Dictionary<SagaStatus, DateTimeOffset> SagaTimes { get; } = new Dictionary<SagaStatus, DateTimeOffset>();
        
        protected SagaState()
        {
            Status = SagaStatus.NotStarted;
            StopWatch();
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