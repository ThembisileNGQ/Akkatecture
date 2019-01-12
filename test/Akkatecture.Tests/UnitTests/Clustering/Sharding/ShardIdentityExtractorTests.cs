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
using Akkatecture.Clustering.Core;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
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
            var message = new CreateTestCommand(aggregateId);

            var extractedIdentity =
                ShardIdentityExtractors.AggregateIdentityExtractor<TestAggregate, TestAggregateId>(message).Item1;

            extractedIdentity.Should().BeEquivalentTo(message.AggregateId.Value);
        }

        [Fact]
        public void AggregateIdentityExtractor_ValidObject_ExtractsObject()
        {
            var aggregateId = TestAggregateId.New;
            var message = new CreateTestCommand(aggregateId);

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
            var message = new CreateTestCommand(aggregateId);

            var extractedIdentity =
                ShardIdentityExtractors.AggregateIdentityExtractor<TestAggregate, TestAggregateId>(message).Item1;

            extractedIdentity.Should().BeEquivalentTo(message.AggregateId.Value);
        }
    }
    
}