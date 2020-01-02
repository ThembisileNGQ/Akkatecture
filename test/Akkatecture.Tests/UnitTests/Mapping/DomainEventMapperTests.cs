// The MIT License (MIT)
//
// Copyright (c) 2018 - 2020 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Linq;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Events;
using Akkatecture.Extensions;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
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
        
        [Fact]
        [Category(Category)]
        public void NonCommittedEvent_WhenRead_IsReturnedUnchanged()
        {
            var message = new CreateTestCommand(TestAggregateId.New,CommandId.New);
            var domainEventReadAdapter = new DomainEventReadAdapter();

            var unchanged = domainEventReadAdapter.FromJournal(message, string.Empty);

            unchanged.Events.Single().As<CreateTestCommand>().GetSourceId().Should().Be(message.GetSourceId());
            unchanged.Events.Single().As<CreateTestCommand>().AggregateId.Should().Be(message.AggregateId);
        }
    }
}