using System.ComponentModel;
using Akkatecture.Extensions;
using Akkatecture.Specifications;
using Akkatecture.TestHelpers;
using Akkatecture.TestHelpers.Specifications;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Specifications
{
    [Category(Categories.Unit)]
    public class SpecificationsTests
    {
        [Fact]
        public void NotSpecification_ReturnsTrue_ForNotSatisfied()
        { 
            var isTrue = new TestSpecifications.IsTrueSpecification();

            var isSatisfiedBy = isTrue.Not().IsSatisfiedBy(false);

            isSatisfiedBy.Should().BeTrue();
        }

        [Fact]
        public void NotSpeficication_ReturnsFalse_ForSatisfied()
        {
            var isTrue = new TestSpecifications.IsTrueSpecification();

            var isSatisfiedBy = isTrue.Not().IsSatisfiedBy(true);

            isSatisfiedBy.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        [InlineData(false, false, true)]
        public void OrSpeficication_ReturnsTrue_Correctly(bool notLeft, bool notRight, bool expectedResult)
        {
            var leftIsTrue = (ISpecification<bool>) new TestSpecifications.IsTrueSpecification();
            var rightIsTrue = (ISpecification<bool>)new TestSpecifications.IsTrueSpecification();
            if (notLeft) leftIsTrue = leftIsTrue.Not();
            if (notRight) rightIsTrue = rightIsTrue.Not();
            var orSpecification = leftIsTrue.Or(rightIsTrue);

            var isSatisfiedBy = orSpecification.IsSatisfiedBy(true);

            isSatisfiedBy.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        public void AndSpeficication_ReturnsTrue_Correctly(bool notLeft, bool notRight, bool expectedResult)
        {
            var leftIsTrue = (ISpecification<bool>)new TestSpecifications.IsTrueSpecification();
            var rightIsTrue = (ISpecification<bool>)new TestSpecifications.IsTrueSpecification();
            if (notLeft) leftIsTrue = leftIsTrue.Not();
            if (notRight) rightIsTrue = rightIsTrue.Not();
            var andSpecification = leftIsTrue.And(rightIsTrue);

            var isSatisfiedBy = andSpecification.IsSatisfiedBy(true);

            isSatisfiedBy.Should().Be(expectedResult);
        }

        /*[Fact]
           //TODO adapt this   
        public void ThrowDomainErrorIfNotStatisfied_Throws_IfNotSatisfied()
        {
            // Arrange
            var isTrue = new TestSpecifications.IsTrueSpecification();

            // Act
            Assert.Throws<DomainError>(() => isTrue.ThrowDomainErrorIfNotSatisfied(false));
        }

        [Fact]
        public void ThrowDomainErrorIfNotStatisfied_DoesNotThrow_IfStatisfied()
        {
            // Arrange
            var isTrue = new TestSpecifications.IsTrueSpecification();

            // Act
            Assert.DoesNotThrow(() => isTrue.ThrowDomainErrorIfNotSatisfied(true));
        }*/
    }
}