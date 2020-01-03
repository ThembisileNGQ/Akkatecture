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
using Akkatecture.Aggregates;
using Akkatecture.Clustering.Core;
using Akkatecture.Commands;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Sagas.Test;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Clustering.Sharding
{
    public class MessageExtractorTests
    {
        //Aggregates
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(55)]
        [InlineData(89)]
        [InlineData(120)]
        [InlineData(500)]
        public void AggregateCommandIdentityExtractor_ValidMessage_ExtractsShardValue(int shardSize)
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var message = new CreateTestCommand(aggregateId, commandId);
            var shardResolver = new MessageExtractor<TestAggregate, TestAggregateId>(shardSize);

            var extractedShard = shardResolver.ShardId(message);
            var extractedShardValue = int.Parse(extractedShard);

            extractedShardValue.Should().BeInRange(0, shardSize);
        }

        [Fact]
        public void AggregateCommandIdentityExtractor_InValidObject_ThrowsArgumentException()
        {
            var message = string.Empty;
            var shardResolver = new MessageExtractor<TestAggregate, TestAggregateId>(10);

            this.Invoking(test => shardResolver.EntityId(message))
                .Should().Throw<ArgumentException>()
                .WithMessage(nameof(message));
        }

        [Fact]
        public void AggregateCommandIdentityExtractor_InValidObject_ThrowsArgumentNullException()
        {
            CreateTestCommand message = null;
            var shardResolver = new MessageExtractor<TestAggregate, TestAggregateId>(10);

            // ReSharper disable once ExpressionIsAlwaysNull
            this.Invoking(test => shardResolver.EntityId(message))
                .Should().Throw<ArgumentNullException>();
        }
        
        //Sagas
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(55)]
        [InlineData(89)]
        [InlineData(120)]
        [InlineData(500)]
        public void AggregateSagaIdentityExtractor_ValidMessage_ExtractsShardValue(int shardSize)
        {
            var aggregateId = TestAggregateId.New;
            var receiverId = TestAggregateId.New;
            var testId = TestId.New;
            var test = new Test(testId);
            var now = DateTimeOffset.UtcNow;
            var aggregateSequenceNumber = 3;
            var message =
                new DomainEvent<TestAggregate, TestAggregateId, TestSentEvent>(
                    aggregateId,
                    new TestSentEvent(test, receiverId),
                    new Metadata(),
                    now,
                    aggregateSequenceNumber);
            var shardResolver = new MessageExtractor<TestSagaManager, TestSaga, TestSagaId, TestSagaLocator>(shardSize);

            var extractedShard = shardResolver.ShardId(message);
            var extractedShardValue = int.Parse(extractedShard);

            extractedShardValue.Should().BeInRange(0, shardSize);
        }

        [Fact]
        public void AggregateSagaIdentityExtractor_InValidObject_ThrowsArgumentException()
        {
            var message = string.Empty;
            var shardResolver = new MessageExtractor<TestSagaManager, TestSaga, TestSagaId, TestSagaLocator>(10);

            this.Invoking(test => shardResolver.EntityId(message))
                .Should().Throw<ArgumentException>()
                .WithMessage(nameof(message));
        }

        [Fact]
        public void AggregateSagaIdentityExtractor_InValidObject_ThrowsArgumentNullException()
        {
            DomainEvent<TestAggregate, TestAggregateId, TestSentEvent> message = null;
            var shardResolver = new MessageExtractor<TestSagaManager, TestSaga, TestSagaId, TestSagaLocator>(10);

            // ReSharper disable once ExpressionIsAlwaysNull
            this.Invoking(test => shardResolver.EntityId(message))
                .Should().Throw<ArgumentNullException>();
        }
    }
}
