using System.ComponentModel;
using Akkatecture.TestHelpers;
using Akkatecture.TestHelpers.Specifications;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Specifications
{
    [Category(Categories.Unit)]
    public class TestSpecificationsTests
    {
        [Fact]
        public void IsTrueSpecification_ReturnsTrue_ForTrue()
        {
            var isTrue = new TestSpecifications.IsTrueSpecification();

            var isSatisfiedBy = isTrue.IsSatisfiedBy(true);

            isSatisfiedBy.Should().BeTrue();
        }

        [Fact]
        public void IsTrueSpecification_ReturnsFalse_ForFalse()
        {
            var isTrue = new TestSpecifications.IsTrueSpecification();

            var isSatisfiedBy = isTrue.IsSatisfiedBy(false);

            isSatisfiedBy.Should().BeFalse();
        }

        [Theory]
        [InlineData(4, 3, false)]
        [InlineData(4, 4, false)]
        [InlineData(4, 5, true)]
        public void IsAboveSpecification_Returns_Correct(int limit, int obj, bool expectedIsSatisfiedBy)
        {
            var isAbove = new TestSpecifications.IsAboveSpecification(limit);

            var isSatisfiedBy = isAbove.IsSatisfiedBy(obj);

            isSatisfiedBy.Should().Be(expectedIsSatisfiedBy);
        }
    }
}