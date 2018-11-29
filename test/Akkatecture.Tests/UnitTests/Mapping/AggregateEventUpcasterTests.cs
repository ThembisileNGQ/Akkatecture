using System;
using System.ComponentModel;
using System.Linq;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Extensions;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Upcasters;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Mapping
{
    public class AggregateEventUpcasterTests
    {
        private const string Category = "Mapping";

        [Fact]
        [Category(Category)]
        public void CommittedEvent_Tagged_ContainsTaggedElements()
        {
            var aggregateEventTagger = new TestAggregateEventUpcaster();
            var aggregateSequenceNumber = 3;
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

            var eventSequence = aggregateEventTagger.FromJournal(committedEvent, string.Empty);
            var upcastedEvent = eventSequence.Events.Single();

            if (upcastedEvent is ICommittedEvent<TestAggregate, TestAggregateId, TestCreatedEventV2> e)
            {
                e.AggregateEvent.GetType().Should().Be<TestCreatedEventV2>();
            }
            else
            {
                false.Should().BeTrue();
            }

        }
    }
}