using System.ComponentModel;
using Akkatecture.Specifications.Provided;
using Akkatecture.TestHelpers;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Specifications
{
    [Category(Categories.Unit)]
    public class ExpressionSpecificationTests
    {
        [Fact]
        public void StringIsRight()
        {
            var specification = new ExpressionSpecification<int>(i => (i > 1 && i < 10) || i == 42);

            var str = specification.ToString();

            str.Should().Be("i => (((i > 1) && (i < 10)) || (i == 42))");
        }

        [Theory]
        [InlineData(42, true)]
        [InlineData(-42, false)]
        public void ExpressionIsEvaluated(int value, bool expectedIsSatisfied)
        {
            var is42 = new ExpressionSpecification<int>(i => i == 42);

            var result = is42.IsSatisfiedBy(value);

            result.Should().Be(expectedIsSatisfied);
        }
    }
}