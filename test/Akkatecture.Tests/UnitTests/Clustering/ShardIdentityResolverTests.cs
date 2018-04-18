using System;
using System.Collections.Generic;
using System.Text;
using Akkatecture.Clustering.Core;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Clustering
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
