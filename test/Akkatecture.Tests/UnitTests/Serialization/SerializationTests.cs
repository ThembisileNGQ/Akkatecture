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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.ExecutionResults;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Extensions;
using Akkatecture.TestHelpers;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Snapshots;
using FluentAssertions;
using Xunit;
using EventId = Akkatecture.Aggregates.EventId;

namespace Akkatecture.Tests.UnitTests.Serialization
{
    public class SerializationTests
    {
        private const string Category = "Serialization";

        [Fact]
        [Category(Category)]
        public void CommittedEvent_AfterSerialization_IsValidAfterDeserialization()
        {
            var aggregateSequenceNumber = 3;
            var aggregateId = TestAggregateId.New;
            var entityId = TestId.New;
            var entity = new Test(entityId);
            var aggregateEvent = new TestAddedEvent(entity);
            var now = DateTimeOffset.UtcNow;
            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{aggregateId.Value}-v{aggregateSequenceNumber}");
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

            committedEvent.SerializeDeserialize().Should().BeEquivalentTo(committedEvent);
        }
        
        [Fact]
        [Category(Category)]
        public void DomainEvent_AfterSerialization_IsValidAfterDeserialization()
        {
            var aggregateSequenceNumber = 3;
            var aggregateId = TestAggregateId.New;
            var entityId = TestId.New;
            var entity = new Test(entityId);
            var aggregateEvent = new TestAddedEvent(entity);
            var now = DateTimeOffset.UtcNow;
            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{aggregateId.Value}-v{aggregateSequenceNumber}");
            var eventMetadata = new Metadata
            {
                Timestamp = now,
                AggregateSequenceNumber = aggregateSequenceNumber,
                AggregateName = typeof(TestAggregate).GetAggregateName().Value,
                AggregateId = aggregateId.Value,
                EventId = eventId
            };
            var domainEvent =
                new DomainEvent<TestAggregate, TestAggregateId, TestAddedEvent>(
                    aggregateId, 
                    aggregateEvent,
                    eventMetadata,
                    now,
                    aggregateSequenceNumber);

            domainEvent.SerializeDeserialize().Should().BeEquivalentTo(domainEvent);
        }
        
        [Fact]
        [Category(Category)]
        public void CommittedSnapshot_AfterSerialization_IsValidAfterDeserialization()
        {
            var aggregateSequenceNumber = 3;
            var aggregateId = TestAggregateId.New;
            var aggregateSnapshot = new TestAggregateSnapshot(Enumerable.Range(0,aggregateSequenceNumber-1).Select(x => new TestAggregateSnapshot.TestModel(Guid.NewGuid())).ToList());
            var now = DateTimeOffset.UtcNow;
            var snapshotId = SnapshotId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Snapshots,
                $"{aggregateId.Value}-v{aggregateSequenceNumber}");
            var snapshotMetadata = new SnapshotMetadata
            {
                SnapshotId = snapshotId,
                AggregateSequenceNumber = aggregateSequenceNumber,
                AggregateName = typeof(TestAggregate).GetAggregateName().Value,
                AggregateId = aggregateId.Value,
            };
            var committedEvent =
                new CommittedSnapshot<TestAggregate, TestAggregateId, TestAggregateSnapshot>(
                    aggregateId, 
                    aggregateSnapshot,
                    snapshotMetadata,
                    now,
                    aggregateSequenceNumber);

            committedEvent.SerializeDeserialize().Should().BeEquivalentTo(committedEvent);
        }

        //TODO : Work Item - https://dev.azure.com/lutando/Akkatecture/_workitems/edit/25/
        /*[Fact] 
        [Category(Category)]
        public void DistinctCommand_AfterSerialization_IsValidAfterDeserialization()
        {
            var aggregateId = TestAggregateId.New;
            var magicNumber = 42;
            var command = new TestDistinctCommand(aggregateId, magicNumber);

            command.SerializeDeserialize().Should().BeEquivalentTo(command);
        }*/
        
        [Fact]
        [Category(Category)]
        public void AddFourTestsCommand_AfterSerialization_IsValidAfterDeserialization()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new AddFourTestsCommand(aggregateId, commandId, new Test(TestId.New));

            command.SerializeDeserialize().Should().BeEquivalentTo(command);
        }
        
        [Fact]
        [Category(Category)]
        public void FailedExecutionResult_AfterSerialization_IsValidAfterDeserialization()
        {
            var failureString = "this is a failed execution";
            var executionResult = new FailedExecutionResult(new List<string>{failureString});

            var result = executionResult.SerializeDeserialize();
            
            result.Should().BeEquivalentTo(executionResult);
            result.Errors.Should().Equal(failureString);
        }
        
        [Fact]
        [Category(Category)]
        public void SuccessfulExecutionResult_AfterSerialization_IsValidAfterDeserialization()
        {
            var executionResult = new SuccessExecutionResult();

            var result = executionResult.SerializeDeserialize();
            
            result.Should().BeEquivalentTo(executionResult);
        }

    }

}