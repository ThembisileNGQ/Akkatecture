using System;
using System.Collections.Generic;
using Akka.TestKit.Xunit2;
using Akkatecture.TestFixtures.Aggregates;
using Akkatecture.TestHelpers.Aggregates;
using FluentAssertions;
using System.ComponentModel;
using System.Linq;
using Akka.Actor;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Snapshots;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Fixtures
{
    [Collection("FixtureTests")]
    public class AggregateFixtureTests
    {
        private const string Category = "AggregateFixture";
        private string _config = TestHelpers.Akka.Configuration.Config;

        [Fact]
        [Category(Category)]
        public void FixtureArrangerWithIdentity_CreatesAggregateRef()
        {
            /*
             *[ERROR][04/08/2019 16:49:36][Thread 0010][akka://fixture-tests1/system/akka.persistence.journal.inmem] Object reference not set to an instance of an object.
Cause: [akka://fixture-tests1/system/akka.persistence.journal.inmem#281960718]: Akka.Actor.ActorInitializationException: Exception during creation ---> System.NullReferenceException: Object reference not set to an instance of an object.
   at Akka.Actor.Props.NewActor()
   at Akka.Actor.ActorCell.CreateNewActorInstance()
   at Akka.Actor.ActorCell.<>c__DisplayClass109_0.<NewActor>b__0()
   at Akka.Actor.ActorCell.UseThreadContext(Action action)
   at Akka.Actor.ActorCell.NewActor()
   at Akka.Actor.ActorCell.Create(Exception failure)
   --- End of inner exception stack trace ---
   at Akka.Actor.ActorCell.Create(Exception failure)
   at Akka.Actor.ActorCell.SysMsgInvokeAll(EarliestFirstSystemMessageList messages, Int32 currentState)
             * 
             */
            using (var testKit = new TestKit(_config, "fixture-tests1"))
            {
                var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(testKit);
                var aggregateIdentity = TestAggregateId.New;

                fixture.For(aggregateIdentity);

                fixture.AggregateRef.Path.Name.Should().Be(aggregateIdentity.Value);
                fixture.AggregateId.Should().Be(aggregateIdentity);
            }
        }

        [Fact]
        [Category(Category)]
        public void FixtureArrangerWithAggregateManager_CreatesAggregateManagerRef()
        {
            using (var testKit = new TestKit(_config,"fixture-tests2"))
            {
                var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(testKit);
                var aggregateIdentity = TestAggregateId.New;

                fixture.Using(() => new TestAggregateManager(), aggregateIdentity);

                fixture.AggregateRef.Path.Name.Should().Be("aggregate-manager");
                fixture.AggregateId.Should().Be(aggregateIdentity);
            }
        }
        
        [Fact]
        [Category(Category)]
        public void FixtureArrangerWithEvents_CanBeReplayed()
        {
            using (var testKit = new TestKit(_config,"fixture-tests3"))
            {
                var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(testKit);
                var aggregateIdentity = TestAggregateId.New;
                var events = new List<IAggregateEvent<TestAggregate, TestAggregateId>>();
                events.Add(new TestCreatedEvent(aggregateIdentity));
                events.AddRange(Enumerable.Range(0, 10).Select(x => new TestAddedEvent(new Test(TestId.New))));
                var journal = Persistence.Instance.Apply(testKit.Sys).JournalFor(null);
                var receiverProbe = testKit.CreateTestProbe("journal-probe");
                fixture
                    .For(aggregateIdentity)
                    .Given(events.ToArray());
                
                
                journal.Tell(new ReplayMessages(1, long.MaxValue, long.MaxValue, aggregateIdentity.ToString(), receiverProbe.Ref));

                
                var from = 1;
                foreach (var _ in events)
                {
                    var index = from;
                    receiverProbe.ExpectMsg<ReplayedMessage>(x =>
                        x.Persistent.SequenceNr == index &&
                        x.Persistent.Payload is ICommittedEvent<TestAggregate, TestAggregateId, IAggregateEvent<TestAggregate,TestAggregateId>>);
                    
                    from++;
                }
                
                receiverProbe.ExpectMsg<RecoverySuccess>();
            }
        } 
        
        [Fact]
        [Category(Category)]
        public void FixtureArrangerWithSnapshot_CanBeLoaded()
        {
            using (var testKit = new TestKit(_config,"fixture-tests4"))
            {
                var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(testKit);
                var aggregateIdentity = TestAggregateId.New;
                var snapshot = new TestAggregateSnapshot(Enumerable.Range(0, 10)
                    .Select(x => new TestAggregateSnapshot.TestModel(Guid.NewGuid())).ToList());
                var snapshotStore = Persistence.Instance.Apply(testKit.Sys).SnapshotStoreFor(null);
                var receiverProbe = testKit.CreateTestProbe("snapshot-probe");
                var snapshotSequenceNumber = 1L;
                fixture
                    .For(aggregateIdentity)
                    .Given(snapshot, snapshotSequenceNumber);
                
                snapshotStore.Tell(new LoadSnapshot(aggregateIdentity.Value, new SnapshotSelectionCriteria(long.MaxValue, DateTime.MaxValue), long.MaxValue), receiverProbe.Ref);
                
                receiverProbe.ExpectMsg<LoadSnapshotResult>(x =>
                    x.Snapshot.Snapshot is ComittedSnapshot<TestAggregate, TestAggregateId, IAggregateSnapshot<TestAggregate, TestAggregateId>> &&
                    x.Snapshot.Metadata.SequenceNr == snapshotSequenceNumber &&
                    x.Snapshot.Metadata.PersistenceId == aggregateIdentity.Value &&
                    x.Snapshot.Snapshot
                        .As<ComittedSnapshot<TestAggregate, TestAggregateId,IAggregateSnapshot<TestAggregate, TestAggregateId>>>().AggregateSnapshot
                        .As<TestAggregateSnapshot>().Tests.Count == snapshot.Tests.Count &&
                    x.ToSequenceNr == long.MaxValue);
                
            }
        }
        
    }
}
