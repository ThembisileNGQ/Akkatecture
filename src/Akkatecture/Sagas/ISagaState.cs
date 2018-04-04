using System;
using System.Collections.Generic;

namespace Akkatecture.Sagas
{
    public interface ISagaState<TSaga, TIdentity>
        where TSaga : ISaga
        where TIdentity : ISagaId
    {
        SagaStatus Status { get; }

        Dictionary<SagaStatus, DateTimeOffset> SagaTimes { get; }

        void Start();

        void Complete();

        void Fail();

        void Cancel();

        void PartiallySucceed();

        void StopWatch();
    }
}