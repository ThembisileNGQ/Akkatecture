using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Subscribers;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Events;

namespace Akkatecture.TestHelpers.Subscribers
{
    public class TestAggregateSubscriber : DomainEventSubscriber,
        ISubscribeTo<TestAggregate,TestAggregateId,TestCreatedEvent>
    {

        public TestAggregateSubscriber()
        {
            ReceiveAsync<IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>>(Handle);
        }
        
        public Task Handle(IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent> domainEvent)
        {
            var handled = new TestSubscribedEventHandled<TestCreatedEvent>(domainEvent.AggregateEvent);
            Context.System.EventStream.Publish(handled);
            return Task.CompletedTask;
        }
    }

    public class TestSubscribedEventHandled<TAggregateEvent> 
    {
        public TAggregateEvent AggregateEvent { get;}

        public TestSubscribedEventHandled(TAggregateEvent aggregateEvent)
        {
            AggregateEvent = aggregateEvent;
        }
    }
}