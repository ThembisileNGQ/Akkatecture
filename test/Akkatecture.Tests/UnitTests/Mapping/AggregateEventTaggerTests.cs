using System;
using System.ComponentModel;
using Akka.Persistence.Journal;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Events;
using Akkatecture.Extensions;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Mapping
{
    public class AggregateEventTaggerTests
    {
        private const string Category = "Mapping";

        [Fact]
        [Category(Category)]
        public void CommittedEvent_WhenTagged_ContainsAggregateNameAsTaggedElement()
        {
            var aggregateEventTagger = new AggregateEventTagger();
            var aggregateSequenceNumber = 3;
            var aggregateId = TestAggregateId.New;
            var entityId = TestId.New;
            var entity = new Test(entityId);
            var aggregateEvent = new TestAddedEvent(entity);
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
                new CommittedEvent<TestAggregate, TestAggregateId, TestAddedEvent>(
                    aggregateId,
                    aggregateEvent,
                    eventMetadata,
                    now,
                    aggregateSequenceNumber);

            var taggedEvent = aggregateEventTagger.ToJournal(committedEvent);

            if (taggedEvent is Tagged a)
            {
                a.Tags.Should().Contain(typeof(TestAggregate).GetAggregateName().Value);
            }
            else
            {
                false.Should().BeTrue();
            }
        }
    }
}