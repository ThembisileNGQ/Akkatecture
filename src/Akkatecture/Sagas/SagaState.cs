// The MIT License (MIT)
//
// Copyright (c) 2015-2018 Rasmus Mikkelsen
// Copyright (c) 2015-2018 eBay Software Foundation
// Modified from original source https://github.com/eventflow/EventFlow
//
// Copyright (c) 2018 Lutando Ngqakaza
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
        public Dictionary<SagaStatus, DateTimeOffset> SagaTimes { get; } 

        static SagaState()
        {
            ApplyMethods = typeof(TEventApplier).GetAggregateEventApplyMethods<TSaga, TIdentity, TEventApplier>();
        }


        protected SagaState()
        {
            SagaTimes = new Dictionary<SagaStatus, DateTimeOffset>();
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