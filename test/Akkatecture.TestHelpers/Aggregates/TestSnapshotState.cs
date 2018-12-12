using System;
using System.Collections.Generic;
using System.Linq;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class TestSnapshotState: SnapshotAggregateState<TestAggregate, TestAggregateId>,
        IApply<TestAddedEvent>,
        IApply<TestReceivedEvent>,
        IApply<TestSentEvent>,
        IApply<TestCreatedEvent>,
        IHydrate<TestSnapshotDataModel>
    {
        public List<Test> TestCollection { get; private set; }

        public void Apply(TestCreatedEvent aggregateEvent)
        {
            TestCollection = new List<Test>();
        }
        
        public void Apply(TestAddedEvent aggregateEvent)
        {
            TestCollection.Add(aggregateEvent.Test);
        }

        public void Apply(TestReceivedEvent aggregateEvent)
        {
            TestCollection.Add(aggregateEvent.Test);
        }

        public void Apply(TestSentEvent aggregateEvent)
        {
            TestCollection.RemoveAll(x => x.Id == aggregateEvent.Test.Id);
        }

        public void Hydrate(TestSnapshotDataModel aggregateSnapshot)
        {
            TestCollection = aggregateSnapshot.Tests.Select(x => new Test(TestId.With(x.Id))).ToList();
        }
    }

    public class TestSnapshotDataModel : IAggregateSnapshot<TestAggregate, TestAggregateId>
    {
        public List<TestDataModel> Tests { get; set; }
        
        
        
        public class TestDataModel
        {
            public Guid Id { get; set; }
        }
    }
}