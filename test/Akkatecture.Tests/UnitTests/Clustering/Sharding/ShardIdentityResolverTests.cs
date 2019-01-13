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
    public class ShardIdentityResolverTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(55)]
        [InlineData(89)]
        [InlineData(120)]
        [InlineData(500)]
        public void AggregateCommandIdentityExtractor_ValidMessage_ExtractsIdentity(int shardSize)
        {
            var aggregateId = TestAggregateId.New;
            var message = new CreateTestCommand(aggregateId);
            var shardResolver = new ShardResolvers(shardSize);

            var extractedShard = shardResolver.AggregateShardResolver<TestAggregate, TestAggregateId>(message);
            var extractedShardValue = int.Parse(extractedShard);

            extractedShardValue.Should().BeInRange(0, shardSize);
        }

        [Fact]
        public void AggregateCommandIdentityExtractor_InValidObject_ThrowsArgumentException()
        {
            var message = string.Empty;
            var shardResolver = new ShardResolvers(10);

            this.Invoking(test => shardResolver.AggregateShardResolver<TestAggregate, TestAggregateId>(message))
                .Should().Throw<ArgumentException>()
                .WithMessage(nameof(message));
        }

        [Fact]
        public void AggregateCommandIdentityExtractor_InValidObject_ThrowsArgumentNullException()
        {
            CreateTestCommand message = null;
            var shardResolver = new ShardResolvers(10);

            // ReSharper disable once ExpressionIsAlwaysNull
            this.Invoking(test => shardResolver.AggregateShardResolver<TestAggregate, TestAggregateId>(message))
                .Should().Throw<ArgumentNullException>();
        }
    }
}
