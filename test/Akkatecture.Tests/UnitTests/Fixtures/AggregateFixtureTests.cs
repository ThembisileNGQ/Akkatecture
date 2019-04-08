using Akka.TestKit.Xunit2;
using Akkatecture.TestFixtures.Aggregates;
using Akkatecture.TestHelpers.Aggregates;
using FluentAssertions;
using System.ComponentModel;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Fixtures
{
    [Collection("FixtureTests")]
    public class AggregateFixtureTests
    {
        private const string Category = "AggregateFixture";

        [Fact]
        [Category(Category)]
        public void FixtureArrangerWithIdentity_CreatesAggregateRef()
        {
            using (var testKit = new TestKit(TestHelpers.Akka.Configuration.Config))
            {
                var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(testKit);
                var aggregateIdentity = TestAggregateId.New;

                fixture.For(aggregateIdentity);

                fixture.AggregateRef.Path.Name.Should().Be(aggregateIdentity.Value);
                fixture.AggregateId.Should().Be(aggregateIdentity);
            };
        }

        [Fact]
        [Category(Category)]
        public void FixtureArrangerWithAggregateManager_CreatesAggregateManagerRef()
        {
            using (var testKit = new TestKit(TestHelpers.Akka.Configuration.Config))
            {
                var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(testKit);
                var aggregateIdentity = TestAggregateId.New;

                fixture.For(aggregateIdentity);

                fixture.AggregateRef.Path.Name.Should().Be("aggregate-manager");
                fixture.AggregateId.Should().Be(aggregateIdentity);
            };
        }
    }
}
