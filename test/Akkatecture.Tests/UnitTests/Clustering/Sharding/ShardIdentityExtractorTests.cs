// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
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
using Akkatecture.TestHelpers.Aggregates.Sagas;
using Akkatecture.TestHelpers.Aggregates.Sagas.Test;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Clustering.Sharding
{
    public class ShardIdentityExtractorTests
    {
        [Fact]
        public void AggregateIdentityExtractor_ValidMessage_ExtractsIdentity()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var message = new CreateTestCommand(aggregateId, commandId);

            var extractedIdentity =
                ShardIdentityExtractors.AggregateIdentityExtractor<TestAggregate, TestAggregateId>(message).Item1;

            extractedIdentity.Should().BeEquivalentTo(message.AggregateId.Value);
        }

        [Fact]
        public void AggregateIdentityExtractor_ValidObject_ExtractsObject()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var message = new CreateTestCommand(aggregateId, commandId);

            var extractedObject = ShardIdentityExtractors.AggregateIdentityExtractor<TestAggregate, TestAggregateId>(message).Item2;

            extractedObject.GetHashCode().Should().Be(message.GetHashCode());
        }

        [Fact]
        public void AggregateIdentityExtractor_InValidObject_ThrowsArgumentException()
        {
            var message = string.Empty;

            this.Invoking(test => ShardIdentityExtractors.AggregateIdentityExtractor<TestAggregate, TestAggregateId>(message))
                .Should().Throw<ArgumentException>()
                .WithMessage(nameof(message));
        }

        [Fact]
        public void AggregateIdentityExtractor_InValidObject_ThrowsArgumentNullException()
        {
            CreateTestCommand message = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            this.Invoking(test => ShardIdentityExtractors.AggregateIdentityExtractor<TestAggregate, TestAggregateId>(message))
                .Should().Throw<ArgumentNullException>();
        }


        [Fact]
        public void AggregateSagaIdentityExtractor_ValidMessage_ExtractsIdentity()
        {
            var aggregateId = TestAggregateId.New;
            var receiverId = TestAggregateId.New;
            var testId = TestId.New;
            var test = new Test(testId);
            var now = DateTimeOffset.UtcNow;
            var aggregateSequenceNumber = 3;
            var domainEvent =
                new DomainEvent<TestAggregate, TestAggregateId, TestSentEvent>(
                    aggregateId,
                    new TestSentEvent(test, receiverId),
                    new Metadata(),
                    now,
                    aggregateSequenceNumber);

            var extractedIdentity =
                ShardIdentityExtractors.AggregateSagaIdentityExtractor<TestSagaManager,TestSaga,TestSagaId, TestSagaLocator>(domainEvent);

            extractedIdentity.Item1.Should().Contain(test.Id.Value);
            extractedIdentity.Item2
                .As<DomainEvent<TestAggregate, TestAggregateId, TestSentEvent>>().AggregateEvent.Test
                .Should().Be(test);
        }
        
        [Fact]
        public void AggregateSagaIdentityExtractor_InValidMessage_ExceptionIsThrown()
        {
            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var message = new CreateTestCommand(aggregateId, commandId);

            this.Invoking(test => ShardIdentityExtractors.AggregateSagaIdentityExtractor<TestSagaManager,TestSaga,TestSagaId, TestSagaLocator>(message))
                .Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void AggregateSagaIdentityExtractor_NullMessage_ExceptionIsThrown()
        {

            this.Invoking(test => ShardIdentityExtractors.AggregateSagaIdentityExtractor<TestSagaManager,TestSaga,TestSagaId, TestSagaLocator>(null))
                .Should().Throw<ArgumentNullException>();
        }
        
        
    }
    
}