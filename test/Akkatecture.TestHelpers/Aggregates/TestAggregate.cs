// The MIT License (MIT)
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

using System.Linq;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Aggregates.Snapshot.Strategies;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Errors;
using Akkatecture.TestHelpers.Aggregates.Events.Signals;

namespace Akkatecture.TestHelpers.Aggregates
{
    [AggregateName("Test")]
    public class TestAggregate : AggregateRoot<TestAggregate, TestAggregateId, TestState>
    {
        public int TestErrors { get; private set; }
        public TestAggregate(TestAggregateId aggregateId)
            : base(aggregateId, new SnapshotEveryFewVersionsStrategy(10))
        {
            TestErrors = 0;
            //Aggregate Commands
            Command<CreateTestCommand>(Execute);
            Command<AddTestCommand>(Execute);
            Command<GiveTestCommand>(Execute);
            Command<ReceiveTestCommand>(Execute);

            //Aggregate Test Probe Commands
            Command<PoisonTestAggregateCommand>(Execute);
            Command<PublishTestStateCommand>(Execute);         
            Command<TestDomainErrorCommand>(Execute);

            Command<SaveSnapshotSuccess>(SnapshotStatus);
            Command<SaveSnapshotFailure>(SnapshotStatus);
        }

        private bool Execute(CreateTestCommand command)
        {
            if (IsNew)
            {
                Emit(new TestCreatedEvent(command.AggregateId));
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
            }

            return true;
        }

        private bool Execute(AddTestCommand command)
        {
            if (!IsNew)
            {

                Emit(new TestAddedEvent(command.Test));

            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
            }
            return true;
        }

        private bool Execute(GiveTestCommand command)
        {
            if (!IsNew)
            {
                if (State.TestCollection.Any(x => x.Id == command.TestToGive.Id))
                {
                    Emit(new TestSentEvent(command.TestToGive,command.ReceiverAggregateId));
                }
                
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
            }

            return true;
        }

        private bool Execute(ReceiveTestCommand command)
        {
            if (!IsNew)
            {
                Emit(new TestReceivedEvent(command.SenderAggregateId,command.TestToReceive));
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
            }

            return true;
        }

        private bool Execute(PoisonTestAggregateCommand command)
        {
            if (!IsNew)
            {
                Context.Stop(Self);
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
            }

            return true;
        }
        
        private bool Execute(PublishTestStateCommand command)
        {
            Signal(new TestStateSignalEvent(State,LastSequenceNr,Version));

            return true;
        }
       
        
        private bool Execute(TestDomainErrorCommand command)
        {
            TestErrors++;
            Throw(new TestedErrorEvent(TestErrors));

            return true;
        }

        protected override bool SnapshotStatus(SaveSnapshotSuccess snapshotSuccess)
        {
            Context.Stop(Self);
            Context.Parent.Tell(new PublishTestStateCommand(Id), Self);
            return true;
        }

        protected override bool SnapshotStatus(SaveSnapshotFailure snapshotFailure)
        {
            return true;
        }

        protected override IAggregateSnapshot<TestAggregate, TestAggregateId> CreateSnapshot()
        {
            return new TestSnapshotDataModel
            {
                Tests = State.TestCollection.Select(x => new TestSnapshotDataModel.TestDataModel {Id = x.Id.GetGuid()})
                    .ToList()
            };
        }
    }
}