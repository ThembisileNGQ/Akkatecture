// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
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
using System.Linq;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Aggregates.Snapshot.Strategies;
using Akkatecture.Core;
using Akkatecture.Extensions;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Errors;
using Akkatecture.TestHelpers.Aggregates.Events.Signals;
using Akkatecture.TestHelpers.Aggregates.Snapshots;

namespace Akkatecture.TestHelpers.Aggregates
{
    [AggregateName("Test")]
    public class TestAggregate : AggregateRoot<TestAggregate, TestAggregateId, TestAggregateState>
    {
        public int TestErrors { get; private set; }
        public TestAggregate(TestAggregateId aggregateId)
            : base(aggregateId)
        {
            TestErrors = 0;
            //Aggregate Commands
            Command<CreateTestCommand>(Execute);
            Command<AddTestCommand>(Execute);
            Command<AddFourTestsCommand>(Execute);
            Command<GiveTestCommand>(Execute);
            Command<ReceiveTestCommand>(Execute);

            //Aggregate Test Probe Commands
            Command<PoisonTestAggregateCommand>(Execute);
            Command<PublishTestStateCommand>(Execute);         
            Command<TestDomainErrorCommand>(Execute);

            Command<SaveSnapshotSuccess>(SnapshotStatus);
            Command<SaveSnapshotFailure>(SnapshotStatus);
            
            SetSnapshotStrategy(new SnapshotEveryFewVersionsStrategy(10));
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
        
        private bool Execute(AddFourTestsCommand command)
        {
            if (!IsNew)
            {
                var events = Enumerable
                    .Range(0, 4)
                    .Select(x => new TestAddedEvent(command.Test));
                
                EmitAll(events);

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
            return new TestAggregateSnapshot
            {
                Tests = State.TestCollection.Select(x => new TestAggregateSnapshot.TestModel{Id = x.Id.GetGuid()})
                    .ToList()
            };
        }
        
        private void Signal<TAggregateEvent>(TAggregateEvent aggregateEvent, IMetadata metadata = null)
            where TAggregateEvent : IAggregateEvent<TestAggregate, TestAggregateId>
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }

            _eventDefinitionService.Load(typeof(TAggregateEvent));
            var eventDefinition = _eventDefinitionService.GetDefinition(typeof(TAggregateEvent));
            var aggregateSequenceNumber = Version;
            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{Id.Value}-v{aggregateSequenceNumber}");
            var now = DateTimeOffset.UtcNow;
            var eventMetadata = new Metadata
            {
                Timestamp = now,
                AggregateSequenceNumber = aggregateSequenceNumber,
                AggregateName = Name.Value,
                AggregateId = Id.Value,
                EventId = eventId,
                EventName = eventDefinition.Name,
                EventVersion = eventDefinition.Version
            };

            eventMetadata.Add(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }

            Logger.Info($"[{Name}] With Id={Id} Commited [{typeof(TAggregateEvent).PrettyPrint()}]");

            var domainEvent = new DomainEvent<TestAggregate, TestAggregateId, TAggregateEvent>(Id, aggregateEvent, eventMetadata, now, Version);

            Publish(domainEvent);
        }

        private void Throw<TAggregateEvent>(TAggregateEvent aggregateEvent, IMetadata metadata = null)
            where TAggregateEvent : IAggregateEvent<TestAggregate, TestAggregateId>
        {
            Signal(aggregateEvent, metadata);
        }
    }
}