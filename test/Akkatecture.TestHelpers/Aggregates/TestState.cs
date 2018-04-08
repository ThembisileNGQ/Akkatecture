using System.Collections.Generic;
using Akkatecture.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class TestState : AggregateState<TestAggregate, TestAggregateId, IEventApplier<TestAggregate, TestAggregateId>>,
        IApply<TestAddedEvent>,
        IApply<TestReceivedEvent>,
        IApply<TestCreatedEvent>
    {
        public Dictionary<TestId, Test> TestCollection ;
        
        public void Apply(TestCreatedEvent aggregateEvent)
        {
            TestCollection = new Dictionary<TestId, Test>();
        }
        
        public void Apply(TestAddedEvent aggregateEvent)
        {
            TestCollection.Add(aggregateEvent.Test.Id,aggregateEvent.Test);
        }

        public void Apply(TestReceivedEvent aggregateEvent)
        {
            TestCollection.Add(aggregateEvent.Test.Id,aggregateEvent.Test);
        }
    }
}