// The MIT License (MIT)
//
// Copyright (c) 2018 - 2020 Lutando Ngqakaza
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
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Akkatecture.Sagas.SagaTimeouts;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Sagas.Test.Events;
using Akkatecture.TestHelpers.Aggregates.Sagas.Test.SagaTimeouts;

namespace Akkatecture.TestHelpers.Aggregates.Sagas.Test
{
    public class TestSaga : AggregateSaga<TestSaga, TestSagaId, TestSagaState>,
        ISagaIsStartedBy<TestAggregate, TestAggregateId, TestSentEvent>,
        ISagaHandles<TestAggregate, TestAggregateId, TestReceivedEvent>,
        ISagaHandlesTimeout<TestSagaTimeout>,
        ISagaHandlesTimeoutAsync<TestSagaTimeout2>
    {
        private IActorRef TestAggregateManager { get; }

        public TestSaga(IActorRef testAggregateManager)
        {
            TestAggregateManager = testAggregateManager;

            Command<EmitTestSagaState>(Handle);
        }

        public bool Handle(IDomainEvent<TestAggregate, TestAggregateId, TestSentEvent> domainEvent)
        {
            if (IsNew)
            {
                var command = new ReceiveTestCommand(
                    domainEvent.AggregateEvent.RecipientAggregateId,
                    CommandId.New,
                    domainEvent.AggregateIdentity,
                    domainEvent.AggregateEvent.Test);
                
                RequestTimeout(new TestSagaTimeout("First timeout test"),
                    TimeSpan.FromSeconds(5));

                RequestTimeout(new TestSagaTimeout2("Second timeout test; handled asynchronously"),
                    TimeSpan.FromSeconds(10));
                
                Emit(new TestSagaStartedEvent(domainEvent.AggregateIdentity,
                    domainEvent.AggregateEvent.RecipientAggregateId, domainEvent.AggregateEvent.Test));

                TestAggregateManager.Tell(command);

            }

            return true;
        }

        public bool Handle(IDomainEvent<TestAggregate, TestAggregateId, TestReceivedEvent> domainEvent)
        {
            if (!IsNew)
            {
                Emit(new TestSagaTransactionCompletedEvent());
                Self.Tell(new EmitTestSagaState());

            }

            return true;
        }

        private bool Handle(EmitTestSagaState testCommmand)
        {
            Emit(new TestSagaCompletedEvent(State));
            return true;
        }

        private class EmitTestSagaState
        {
        }

        public bool HandleTimeout(TestSagaTimeout timeout)
        {
            var message = ((TestSagaTimeout) timeout).MessageToInclude;
            Emit(new TestSagaTimeoutOccurred(message));
            return true;
        }

        public Task HandleTimeoutAsync(TestSagaTimeout2 timeout)
        {
            var message = ((TestSagaTimeout2) timeout).MessageToInclude;
            Emit(new TestSagaTimeoutOccurred(message));
            return Task.CompletedTask;
        }
    }
}
