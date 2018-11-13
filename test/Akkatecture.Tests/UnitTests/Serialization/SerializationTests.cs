using System;
using System.ComponentModel;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Extensions;
using Akkatecture.TestHelpers;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Serialization
{
    public class SerializationTests
    {
        private const string Category = "Serialization";

        [Fact]
        [Category(Category)]
        public void CommittedEvent_AfterSerialization_IsValidAfterDeserialization()
        {
            
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
                AggregateSequenceNumber = 3,
                AggregateName = typeof(TestAggregate).GetAggregateName().Value,
                AggregateId = aggregateId.Value,
                EventId = eventId
            };
            var committedEvent =
                new CommittedEvent<TestAggregate, TestAggregateId, TestAddedEvent>(
                    aggregateId, 
                    aggregateEvent,
                    eventMetadata);

            committedEvent.SerializeDeserialize().Should().BeEquivalentTo(committedEvent);
        }
        
    }

}