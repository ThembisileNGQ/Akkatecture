using Akkatecture.Extensions;
using Akkatecture.TestHelpers.Specifications;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Specifications
{
    public class ProvidedSpecificationsTests
    {
        [Theory]
        [InlineData(1, 1, false)]
        [InlineData(1, 2, true)]
        [InlineData(1, 3, true)]
        [InlineData(1, 4, true)]
        [InlineData(1, 5, true)]
        [InlineData(3, 1, false)]
        [InlineData(3, 2, false)]
        [InlineData(3, 3, false)]
        [InlineData(3, 4, true)]
        [InlineData(3, 5, true)]
        public void AtLeast_Returns_Correctly(int requiredSpecifications, int obj, bool expectedIsSatisfiedBy)
        {
            // Arrange
            var isAbove1 = new TestSpecifications.IsAboveSpecification(1);
            var isAbove2 = new TestSpecifications.IsAboveSpecification(2);
            var isAbove3 = new TestSpecifications.IsAboveSpecification(3);
            var isAbove4 = new TestSpecifications.IsAboveSpecification(4);
            var atLeast = new[] { isAbove1, isAbove2, isAbove3, isAbove4 }.AtLeast(requiredSpecifications);

            // Act
            var isSatisfiedBy = atLeast.IsSatisfiedBy(obj);

            // Assert
            isSatisfiedBy.Should().Be(expectedIsSatisfiedBy, string.Join(", ", atLeast.WhyIsNotSatisfiedBy(obj)));
        }
    }
}