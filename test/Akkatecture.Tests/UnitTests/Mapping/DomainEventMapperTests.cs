using System;
using System.ComponentModel;
using System.Linq;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Events;
using Akkatecture.Extensions;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Events;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Mapping
{
    public class DomainEventMapperTests
    {
        private const string Category = "Mapping";

        [Fact]
        [Category(Category)]
        public void CommittedEvent_MappedToDomainEvent_IsValid()
        {
            var domainEventReadAdapter = new DomainEventReadAdapter();
            var aggregateSequenceNumber = 5;
            var aggregateId = TestAggregateId.New;
            var aggregateEvent = new TestCreatedEvent(aggregateId);
            var now = DateTimeOffset.UtcNow;
            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{aggregateId.Value}-v{3}");
            var eventMetadata = new Metadata
            {
                Timestamp = now,
                AggregateSequenceNumber = aggregateSequenceNumber,
                AggregateName = typeof(TestAggregate).GetAggregateName().Value,
                AggregateId = aggregateId.Value,
                EventId = eventId
            };
            var committedEvent =
                new CommittedEvent<TestAggregate, TestAggregateId, TestCreatedEvent>(
                    aggregateId,
                    aggregateEvent,
                    eventMetadata,
                    now,
                    aggregateSequenceNumber);

            var eventSequence = domainEventReadAdapter.FromJournal(committedEvent, string.Empty);
            var upcastedEvent = eventSequence.Events.Single();

            if (upcastedEvent is IDomainEvent<TestAggregate, TestAggregateId, TestCreatedEvent> e)
            {
                e.AggregateEvent.GetType().Should().Be<TestCreatedEvent>();
            }
            else
            {
                false.Should().BeTrue();
            }

        }
    }
}