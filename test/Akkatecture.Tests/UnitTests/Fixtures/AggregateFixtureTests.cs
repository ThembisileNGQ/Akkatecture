using System;
using System.Collections.Generic;
using Akka.TestKit.Xunit2;
using Akkatecture.TestFixture.Aggregates;
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
        private readonly string _config = TestHelpers.Akka.Configuration.Config;

        [Fact]
        [Category(Category)]
        public void FixtureArrangerWithIdentity_CreatesAggregateRef()
        {
            //TODO - https://dev.azure.com/lutando/Akkatecture/_workitems/edit/23/
            using (var testKit = new TestKit(_config, "fixture-tests-1"))
            {
                var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(testKit);
                var aggregateIdentity = TestAggregateId.New;

                fixture
                    .For(aggregateIdentity)
                    .GivenNothing();

                fixture.AggregateRef.Path.Name.Should().Be(aggregateIdentity.Value);
                fixture.AggregateId.Should().Be(aggregateIdentity);
            }
        }

        [Fact]
        [Category(Category)]
        public void FixtureArrangerWithAggregateManager_CreatesAggregateManagerRef()
        {
            using (var testKit = new TestKit(_config,"fixture-tests-2"))
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
            using (var testKit = new TestKit(_config,"fixture-tests-3"))
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
            using (var testKit = new TestKit(_config,"fixture-tests-4"))
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
                    x.Snapshot.Snapshot is CommittedSnapshot<TestAggregate, TestAggregateId, IAggregateSnapshot<TestAggregate, TestAggregateId>> &&
                    x.Snapshot.Metadata.SequenceNr == snapshotSequenceNumber &&
                    x.Snapshot.Metadata.PersistenceId == aggregateIdentity.Value &&
                    x.Snapshot.Snapshot
                        .As<CommittedSnapshot<TestAggregate, TestAggregateId,IAggregateSnapshot<TestAggregate, TestAggregateId>>>().AggregateSnapshot
                        .As<TestAggregateSnapshot>().Tests.Count == snapshot.Tests.Count &&
                    x.ToSequenceNr == long.MaxValue);
                
            }
        }
        
    }
}
