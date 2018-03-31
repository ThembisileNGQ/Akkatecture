using System.Reflection.Metadata;
using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Subscribers;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Events;

namespace Akkatecture.TestHelpers.Subscribers
{
    public class TestAggregateSubscriber : DomainEventSubscriber<TestAggregate,TestId,AggregateEvent<TestAggregate, TestId>>,
        ISubscribeTo<TestAggregate,TestId,TestCreatedEvent>
    {

        public TestAggregateSubscriber()
        {
            ReceiveAsync<IDomainEvent<TestAggregate, TestId, TestCreatedEvent>>(Handle);
        }
        
        public Task Handle(IDomainEvent<TestAggregate, TestId, TestCreatedEvent> domainEvent)
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