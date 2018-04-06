using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Subscribers;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Events;

namespace Akkatecture.TestHelpers.Subscribers
{
    public class TestAggregateSubscriber : DomainEventSubscriber<TestAggregate,TestAggregateId,AggregateEvent<TestAggregate, TestAggregateId>>,
        ISubscribeTo<TestAggregate,TestAggregateId,TestCreatedEvent>
    {

        public TestAggregateSubscriber()
        {
            ReceiveAsync<IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent>>(Handle);
        }
        
        public Task Handle(IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent> domainEvent)
        {
            var handled = new Handled<TestCreatedEvent>(domainEvent.AggregateEvent);
            Context.System.EventStream.Publish(handled);
            return Task.CompletedTask;
        }
    }

    public class Handled<T> 
    {
        public T AggregateEvent { get;}

        public Handled(T aggregateEvent)
        {
            AggregateEvent = aggregateEvent;
        }
    }
}