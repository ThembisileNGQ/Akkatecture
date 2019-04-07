using System;
using Akka.Actor;
using Akka.Persistence;
using Akka.TestKit;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Sagas.AggregateSaga;
using SnapshotMetadata = Akkatecture.Aggregates.Snapshot.SnapshotMetadata;
using AkkaSnapshotMetadata = Akka.Persistence.SnapshotMetadata;

namespace Akkatecture.TestFixtures.Aggregates
{
    
    public class AggregateFixture<TAggregate, TIdentity> : IFixtureArranger<TAggregate, TIdentity> , IFixtureExecutor<TAggregate,TIdentity> , IFixtureAsserter<TAggregate, TIdentity>
        where TAggregate : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        private readonly TestKitBase _testKit;
        public TIdentity AggregateId { get; private set; }
        public IActorRef AggregateRef { get; private set; }
        public TestProbe AggregateTestProbe { get; private set; }
        
        public AggregateFixture(
            TestKitBase testKit)
        {
            _testKit = testKit;
        }


        public IFixtureArranger<TAggregate, TIdentity> For(TIdentity aggregateId)
        {
            if(aggregateId == null)
                throw new ArgumentNullException(nameof(aggregateId));
            
            AggregateId = aggregateId;
            AggregateTestProbe = _testKit.CreateTestProbe("aggregate-probe");
            AggregateRef = _testKit.Sys.ActorOf(Props.Create<TAggregate>(AggregateId), AggregateId.Value);
            return this;
        }

        public IFixtureExecutor<TAggregate, TIdentity> GivenNothing()
        {
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

        public IFixtureExecutor<TAggregate, TIdentity> Given(ICommand<TAggregate, TIdentity> command)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));
            
            return Given(command);
        }
        public IFixtureExecutor<TAggregate, TIdentity> Given(params ICommand<TAggregate, TIdentity>[] commands)
        {
            if(commands == null)
                throw new ArgumentNullException(nameof(commands));
            
            foreach (var command in commands)
            {
                AggregateRef.Tell(command);
            }
            return this;
        }

        public IFixtureAsserter<TAggregate, TIdentity> When(ICommand<TAggregate, TIdentity> command)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));
            
            AggregateRef.Tell(command);
            return this;
        }
        
        public IFixtureAsserter<TAggregate, TIdentity> ThenExpect<TAggregateEvent>(Predicate<TAggregateEvent> aggregateEventPredicate = null)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            _testKit.Sys.EventStream.Subscribe(AggregateTestProbe, typeof(DomainEvent<TAggregate, TIdentity, TAggregateEvent>));
            
            if(aggregateEventPredicate == null)
                AggregateTestProbe.ExpectMsg<DomainEvent<TAggregate, TIdentity, TAggregateEvent>>();
            else
                AggregateTestProbe.ExpectMsg<DomainEvent<TAggregate, TIdentity, TAggregateEvent>>(x => aggregateEventPredicate(x.AggregateEvent));
            return this;
        }
        
        private void InitializeEventJournal(TIdentity aggregateId, params IAggregateEvent<TAggregate, TIdentity>[] events)
        {
            var writerGuid = Guid.NewGuid().ToString();
            var writes = new AtomicWrite[events.Length];
            for (var i = 0; i < events.Length; i++)
            {
                var comittedEvent = new CommittedEvent<TAggregate, TIdentity, IAggregateEvent<TAggregate, TIdentity>>(aggregateId, events[i], new Metadata(), DateTimeOffset.UtcNow, i+1);
                writes[i] = new AtomicWrite(new Persistent(comittedEvent, i+1, aggregateId.Value, string.Empty, false, ActorRefs.NoSender, writerGuid));
            }
            var journal = Persistence.Instance.Apply(_testKit.Sys).JournalFor(null);
            journal.Tell(new WriteMessages(writes, AggregateTestProbe.Ref, 1));

            AggregateTestProbe.ExpectMsg<WriteMessagesSuccessful>(x =>
            {
                _testKit.Sys.Log.Info($"{aggregateId} journal write message successful with {x}");
                return true;
            });
            
            for (int i = 0; i < events.Length; i++)
                AggregateTestProbe.ExpectMsg<WriteMessageSuccess>(x =>
                {
                    _testKit.Sys.Log.Info($"{aggregateId} journal initialized with {x}");
                    return x.Persistent.PersistenceId == aggregateId.ToString() &&
                           x.Persistent.Payload is CommittedEvent<TAggregate, TIdentity, IAggregateEvent<TAggregate, TIdentity>> &&
                           x.Persistent.SequenceNr == (long) i+1;
                });
        }
        
        private void InitializeSnapshotStore<TAggregateSnapshot>(TIdentity aggregateId, TAggregateSnapshot aggregateSnapshot, long sequenceNumber)
            where TAggregateSnapshot : IAggregateSnapshot<TAggregate, TIdentity>
        {
            var snapshotStore = Persistence.Instance.Apply(_testKit.Sys).SnapshotStoreFor(null);
            var comittedSnapshot = new ComittedSnapshot<TAggregate, TIdentity, TAggregateSnapshot>(aggregateId, aggregateSnapshot, new SnapshotMetadata(), DateTimeOffset.UtcNow, sequenceNumber);
            
            var metadata = new AkkaSnapshotMetadata(aggregateId.ToString(), sequenceNumber);
            snapshotStore.Tell(new SaveSnapshot(metadata, comittedSnapshot), AggregateTestProbe.Ref);

            AggregateTestProbe.ExpectMsg<SaveSnapshotSuccess>(x =>
            {
                _testKit.Sys.Log.Info($"{aggregateId} store write message successful with {x}");
                return x.Metadata.SequenceNr == sequenceNumber &&
                       x.Metadata.PersistenceId == aggregateId.ToString();
            });
            
        }
        
    }
}