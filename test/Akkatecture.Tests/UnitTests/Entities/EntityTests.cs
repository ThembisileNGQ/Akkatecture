using System;
using Akkatecture.TestHelpers.Aggregates.Entities;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Entities
{
    public class EntityTests
    {
        [Fact]
        public void InstantiatingEntity_WithNullId_ThrowsException()
        {
            this.Invoking(test => new Test(null))
                .Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void InstantiatingEntity_WithValidId_HasIdentity()
        {
            var testId = TestId.New;
            
            var test = new Test(testId);

            test.GetIdentity().Should().Be(testId);
        }
    }
}