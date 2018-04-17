using System.Collections.Generic;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;

namespace Akkatecture.TestHelpers.Aggregates
{

    public class TestState : AggregateState<TestAggregate, TestAggregateId>,
        IApply<TestAddedEvent>,
        IApply<TestReceivedEvent>,
        IApply<TestSentEvent>,
        IApply<TestCreatedEvent>
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
    }
}