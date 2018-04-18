using System;
using Akkatecture.Clustering.Core;
using Akkatecture.Core;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Clustering
{
    public class ShardIdentityExtractorTests
    {
        [Fact]
        public void AggregateCommandIdentityExtractor_ValidMessage_ExtractsIdentity()
        {
            var aggregateId = TestAggregateId.New;
            var message = new CreateTestCommand(aggregateId);

            var extractedIdentity =
                ShardIdentityExtractors.AggregateCommandIdentityExtractor<TestAggregate, TestAggregateId>(message).Item1;

            extractedIdentity.Should().BeEquivalentTo(message.AggregateId.Value);
        }

        [Fact]
        public void AggregateCommandIdentityExtractor_ValidObject_ExtractsObject()
        {
            var aggregateId = TestAggregateId.New;
            var message = new CreateTestCommand(aggregateId);

            var extractedObject = ShardIdentityExtractors.AggregateCommandIdentityExtractor<TestAggregate, TestAggregateId>(message).Item2;

            extractedObject.GetHashCode().Should().Be(message.GetHashCode());
        }

        [Fact]
        public void AggregateCommandIdentityExtractor_InValidObject_ThrowsArgumentException()
        {
            var message = string.Empty;

            this.Invoking(test => ShardIdentityExtractors.AggregateCommandIdentityExtractor<TestAggregate, TestAggregateId>(message))
                .Should().Throw<ArgumentException>()
                .WithMessage(nameof(message));
        }

        [Fact]
        public void AggregateCommandIdentityExtractor_InValidObject_ThrowsArgumentNullException()
        {
            CreateTestCommand message = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            this.Invoking(test => ShardIdentityExtractors.AggregateCommandIdentityExtractor<TestAggregate, TestAggregateId>(message))
                .Should().Throw<ArgumentNullException>();
        }
    }
    
}