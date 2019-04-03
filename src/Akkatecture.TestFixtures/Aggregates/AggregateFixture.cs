using System;
using Akka.Actor;
using Akka.Persistence;
using Akka.TestKit;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;

namespace Akkatecture.TestFixtures.Aggregates
{
    
    public class AggregateFixture<TAggregate,TIdentity> : IFixtureArranger<TAggregate,TIdentity> , IFixtureExecutor<TAggregate,TIdentity> , IFixtureAsserter<TAggregate, TIdentity>
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

        public IFixtureExecutor<TAggregate, TIdentity> Given<TAggregateEvent>(
            params TAggregateEvent[] aggregateEvents)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            InitializeJournal(AggregateId, aggregateEvents);
            return this;
        }

        public IFixtureExecutor<TAggregate, TIdentity> GivenCommands<TCommand>(params TCommand[] commands) where TCommand : IAggregateEvent<TAggregate, TIdentity>
        {
            throw new NotImplementedException();
        }

        public IFixtureAsserter<TAggregate, TIdentity> When<TCommand>(TCommand command) 
            where TCommand : ICommand<TAggregate, TIdentity>
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));
            
            AggregateRef.Tell(command);
            return this;
        }
        
        public IFixtureAsserter<TAggregate, TIdentity> ThenExpect<TAggregateEvent>(Predicate<TAggregateEvent> aggregateEventPredicate)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            _testKit.Sys.EventStream.Subscribe(AggregateTestProbe, typeof(DomainEvent<TAggregate, TIdentity, TAggregateEvent>));
            AggregateTestProbe.ExpectMsg<DomainEvent<TAggregate, TIdentity, TAggregateEvent>>(x => aggregateEventPredicate(x.AggregateEvent));
            return this;
        }
        
        private void InitializeJournal<TAggregateEvent>(TIdentity aggregateId, params TAggregateEvent[] events)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            var probe = _testKit.CreateTestProbe();
            var writerGuid = Guid.NewGuid().ToString();
            var writes = new AtomicWrite[events.Length];
            for (var i = 0; i < events.Length; i++)
            {
                var e = new CommittedEvent<TAggregate,TIdentity,TAggregateEvent>(aggregateId,events[i],new Metadata(), DateTimeOffset.UtcNow, i);
                writes[i] = new AtomicWrite(new Persistent(e, i+1, aggregateId.Value, "", false, ActorRefs.NoSender, writerGuid));
            }
            var journal = Persistence.Instance.Apply(_testKit.Sys).JournalFor(null);
            journal.Tell(new WriteMessages(writes, probe.Ref, 1));

            probe.ExpectMsg<WriteMessagesSuccessful>();
            for (int i = 0; i < events.Length; i++)
                probe.ExpectMsg<WriteMessageSuccess>();
        }

    }
}