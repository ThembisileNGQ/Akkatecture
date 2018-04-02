using System;
using System.Collections.Generic;
using Akka.Dispatch.SysMsg;

namespace Akkatecture.Sagas
{
    public abstract class SagaState
    {
        public SagaStatus Status { get; private set; }
        private Dictionary<SagaStatus, DateTimeOffset> SagaTimes = new Dictionary<SagaStatus, DateTimeOffset>();
        
        public SagaState()
        {
            Status = SagaStatus.NotStarted;
        }

        public void Start()
        {
            Status = SagaStatus.Running;
        }

        public void Complete()
        {
            Status = SagaStatus.Completed;
            StopWatch();
        }

        public void Fail()
        {
            Status = SagaStatus.Failed;
            StopWatch();
        }

        public void Cancel()
        {
            Status = SagaStatus.Cancelled;
            StopWatch();
        }

        public void PartiallySucceed()
        {
            Status = SagaStatus.PartiallySucceeded;
            StopWatch();
        }

        private void StopWatch()
        {
            if (!SagaTimes.ContainsKey(Status))
            {
                SagaTimes.Add(Status,DateTimeOffset.UtcNow);
            }
        }
    }
}