using System;
using System.Collections.Generic;

namespace Akkatecture.Sagas
{
    public interface ISagaState<out TIdentity>
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