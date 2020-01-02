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
using System.Linq.Expressions;
using Akka.Actor;
using Akka.Persistence;
using Akka.TestKit;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Commands;
using Akkatecture.Core;
using SnapshotMetadata = Akkatecture.Aggregates.Snapshot.SnapshotMetadata;
using AkkaSnapshotMetadata = Akka.Persistence.SnapshotMetadata;

namespace Akkatecture.TestFixture.Aggregates
{
    
    public class AggregateFixture<TAggregate, TIdentity> : IFixtureArranger<TAggregate, TIdentity> , IFixtureExecutor<TAggregate,TIdentity> , IFixtureAsserter<TAggregate, TIdentity>
        where TAggregate : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        private readonly TestKitBase _testKit;
        public TIdentity AggregateId { get; private set; }
        public IActorRef AggregateRef { get; private set; }
        public TestProbe AggregateEventTestProbe { get; private set; }
        public TestProbe AggregateReplyTestProbe { get; private set; }
        public Props AggregateProps { get; private set; }
        public bool UsesAggregateManager { get; private set; }
        public AggregateFixture(
            TestKitBase testKit)
        {
            _testKit = testKit;
        }


        public IFixtureArranger<TAggregate, TIdentity> For(TIdentity aggregateId)
        {
            if(aggregateId == null)
                throw new ArgumentNullException(nameof(aggregateId));
            
            if(!AggregateEventTestProbe.IsNobody())
                throw new InvalidOperationException(nameof(AggregateEventTestProbe));
            
            AggregateId = aggregateId;
            AggregateEventTestProbe = _testKit.CreateTestProbe("aggregate-event-test-probe");
            AggregateReplyTestProbe = _testKit.CreateTestProbe("aggregate-reply-test-probe");
            AggregateProps = Props.Create<TAggregate>(args: aggregateId);
            AggregateRef = ActorRefs.Nobody;
            UsesAggregateManager = false;
            
            return this;
        }

        public IFixtureArranger<TAggregate, TIdentity> Using<TAggregateManager>(
            Expression<Func<TAggregateManager>> aggregateManagerFactory, TIdentity aggregateId)
            where TAggregateManager : ReceiveActor, IAggregateManager<TAggregate, TIdentity>
        {
            if(aggregateId == null)
                throw new ArgumentNullException(nameof(aggregateId));
            if(!AggregateEventTestProbe.IsNobody())
                throw new InvalidOperationException(nameof(AggregateEventTestProbe));
            if(!AggregateReplyTestProbe.IsNobody())
                throw new InvalidOperationException(nameof(AggregateReplyTestProbe));
            
            AggregateId = aggregateId;
            AggregateEventTestProbe = _testKit.CreateTestProbe("aggregate-event-test-probe");
            AggregateReplyTestProbe = _testKit.CreateTestProbe("aggregate-reply-test-probe");
            AggregateRef = _testKit.Sys.ActorOf(Props.Create(aggregateManagerFactory), "aggregate-manager");
            UsesAggregateManager = false;
            AggregateProps = Props.Empty;
            
            return this;
        }

        public IFixtureExecutor<TAggregate, TIdentity> GivenNothing()
        {
            if (!UsesAggregateManager && AggregateRef.IsNobody())
                AggregateRef = _testKit.Sys.ActorOf(AggregateProps, AggregateId.Value);

            return this;
        }

        public IFixtureExecutor<TAggregate, TIdentity> Given(params IAggregateEvent<TAggregate,TIdentity>[] aggregateEvents)
        {
            InitializeEventJournal(AggregateId, aggregateEvents);
            
            return this;
        }
        
        public IFixtureExecutor<TAggregate, TIdentity> Given(IAggregateSnapshot<TAggregate,TIdentity> aggregateSnapshot, long snapshotSequenceNumber)
        {
            InitializeSnapshotStore(AggregateId, aggregateSnapshot, snapshotSequenceNumber);
            
            return this;
        }

        public IFixtureExecutor<TAggregate, TIdentity> Given(params ICommand<TAggregate, TIdentity>[] commands)
        {
            if(commands == null)
                throw new ArgumentNullException(nameof(commands));

            if (!UsesAggregateManager && AggregateRef.IsNobody())
                AggregateRef = _testKit.Sys.ActorOf(AggregateProps, AggregateId.Value);

            foreach (var command in commands)
            {
                if(command == null)
                    throw new NullReferenceException(nameof(command));
                
                AggregateRef.Tell(command, AggregateReplyTestProbe);
            }
            
            return this;
        }

        public IFixtureAsserter<TAggregate, TIdentity> When(params ICommand<TAggregate, TIdentity>[] commands)
        {

            if(commands == null)
                throw new ArgumentNullException(nameof(commands));

            if(!UsesAggregateManager && AggregateRef.IsNobody())
                AggregateRef = _testKit.Sys.ActorOf(AggregateProps, AggregateId.Value);

            foreach (var command in commands)
            {
                if(command == null)
                    throw new NullReferenceException(nameof(command));
                
                AggregateRef.Tell(command, AggregateReplyTestProbe);
            }
            
            return this;
        }

        public IFixtureAsserter<TAggregate, TIdentity> AndWhen(params ICommand<TAggregate, TIdentity>[] commands)
        {
            return When(commands);
        }
        
        public IFixtureAsserter<TAggregate, TIdentity> ThenExpect<TAggregateEvent>(Predicate<TAggregateEvent> aggregateEventPredicate = null)
            where TAggregateEvent : class, IAggregateEvent<TAggregate, TIdentity>
        {
            _testKit.Sys.EventStream.Subscribe(AggregateEventTestProbe, typeof(IDomainEvent<TAggregate, TIdentity, TAggregateEvent>));
            
            if(aggregateEventPredicate == null)
                AggregateEventTestProbe.ExpectMsg<DomainEvent<TAggregate, TIdentity, TAggregateEvent>>();
            else
                AggregateEventTestProbe.ExpectMsg<DomainEvent<TAggregate, TIdentity, TAggregateEvent>>(x => aggregateEventPredicate(x.AggregateEvent));
            
            return this;
        }

        public IFixtureAsserter<TAggregate, TIdentity> ThenExpectReply<TReply>(Predicate<TReply> aggregateReplyPredicate = null)
        {
            AggregateReplyTestProbe.ExpectMsg<TReply>(aggregateReplyPredicate);
            return this;
        }
        
        public IFixtureAsserter<TAggregate, TIdentity> ThenExpectDomainEvent<TAggregateEvent>(Predicate<IDomainEvent<TAggregate, TIdentity, TAggregateEvent>> domainEventPredicate = null)
            where TAggregateEvent : class, IAggregateEvent<TAggregate,TIdentity>
        {
            _testKit.Sys.EventStream.Subscribe(AggregateEventTestProbe, typeof(IDomainEvent<TAggregate, TIdentity, TAggregateEvent>));
            
            if(domainEventPredicate == null)
                AggregateEventTestProbe.ExpectMsg<DomainEvent<TAggregate, TIdentity, TAggregateEvent>>();
            else
                AggregateEventTestProbe.ExpectMsg<DomainEvent<TAggregate, TIdentity, TAggregateEvent>>(domainEventPredicate);
            
            return this;
        }
        
        private void InitializeEventJournal(TIdentity aggregateId, params IAggregateEvent<TAggregate, TIdentity>[] events)
        {
            var writerGuid = Guid.NewGuid().ToString();
            var writes = new AtomicWrite[events.Length];
            for (var i = 0; i < events.Length; i++)
            {
                var committedEvent = new CommittedEvent<TAggregate, TIdentity, IAggregateEvent<TAggregate, TIdentity>>(aggregateId, events[i], new Metadata(), DateTimeOffset.UtcNow, i+1);
                writes[i] = new AtomicWrite(new Persistent(committedEvent, i+1, aggregateId.Value, string.Empty, false, ActorRefs.NoSender, writerGuid));
            }
            var journal = Persistence.Instance.Apply(_testKit.Sys).JournalFor(null);
            journal.Tell(new WriteMessages(writes, AggregateEventTestProbe.Ref, 1));

            AggregateEventTestProbe.ExpectMsg<WriteMessagesSuccessful>();
            
            for (var i = 0; i < events.Length; i++)
            {
                var seq = i;
                AggregateEventTestProbe.ExpectMsg<WriteMessageSuccess>(x =>
                    x.Persistent.PersistenceId == aggregateId.ToString() &&
                    x.Persistent.Payload is CommittedEvent<TAggregate, TIdentity, IAggregateEvent<TAggregate, TIdentity>> &&
                    x.Persistent.SequenceNr == (long) seq+1);
            }
        }
        
        private void InitializeSnapshotStore<TAggregateSnapshot>(TIdentity aggregateId, TAggregateSnapshot aggregateSnapshot, long sequenceNumber)
            where TAggregateSnapshot : IAggregateSnapshot<TAggregate, TIdentity>
        {
            var snapshotStore = Persistence.Instance.Apply(_testKit.Sys).SnapshotStoreFor(null);
            var committedSnapshot = new CommittedSnapshot<TAggregate, TIdentity, TAggregateSnapshot>(aggregateId, aggregateSnapshot, new SnapshotMetadata(), DateTimeOffset.UtcNow, sequenceNumber);
            
            var metadata = new AkkaSnapshotMetadata(aggregateId.ToString(), sequenceNumber);
            snapshotStore.Tell(new SaveSnapshot(metadata, committedSnapshot), AggregateEventTestProbe.Ref);

            AggregateEventTestProbe.ExpectMsg<SaveSnapshotSuccess>(x =>
                x.Metadata.SequenceNr == sequenceNumber &&
                x.Metadata.PersistenceId == aggregateId.ToString());
            
        }
        
    }
}