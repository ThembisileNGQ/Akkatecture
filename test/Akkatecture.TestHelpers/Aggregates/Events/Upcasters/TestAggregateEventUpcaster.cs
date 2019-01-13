using Akkatecture.Events;

namespace Akkatecture.TestHelpers.Aggregates.Events.Upcasters
{
    public class TestAggregateEventUpcaster : AggregateEventUpcaster<TestAggregate,TestAggregateId>,
        IUpcast<TestCreatedEvent,TestCreatedEventV2>
    {
        public TestCreatedEventV2 Upcast(TestCreatedEvent aggregateEvent)
        {
            return new TestCreatedEventV2(aggregateEvent.TestAggregateId, "newArgs");
        }
    }
}
