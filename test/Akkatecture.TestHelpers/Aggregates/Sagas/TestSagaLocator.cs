using System;
using Akkatecture.Aggregates;
using Akkatecture.Sagas;
using Akkatecture.TestHelpers.Aggregates.Events;

namespace Akkatecture.TestHelpers.Aggregates.Sagas
{
    public class TestSagaLocator : ISagaLocator<TestSagaId>
    {
        public TestSagaId LocateSaga(IDomainEvent domainEvent)
        {
            var moniker = "testSaga";
            switch (domainEvent.GetAggregateEvent())
            {
                case TestSentEvent evt:
                    return new TestSagaId($"{moniker}-{evt.Test.Id}");

                case TestReceivedEvent evt:
                    return new TestSagaId($"{moniker}-{evt.Test.Id}");

                default:
                    throw new ArgumentException(nameof(domainEvent));
            }
        }
    }
}
