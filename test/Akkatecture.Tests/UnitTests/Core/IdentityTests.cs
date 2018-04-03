using System;
using System.ComponentModel;
using Akkatecture.TestHelpers;
using Akkatecture.TestHelpers.Aggregates;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Core
{
    [Category(Categories.Unit)]
    public class IdentityTests
    {
        [Fact]
        public void NewDeterministic_ReturnsKnownResult()
        {
            var namespaceId = Guid.Parse("769077C6-F84D-46E3-AD2E-828A576AAAF3");
            const string name = "fantastic 4";

            var testId = TestId.NewDeterministic(namespaceId, name);

            testId.Value.Should().Be("test-da7ab6b1-c513-581f-a1a0-7cdf17109deb");
            TestId.IsValid(testId.Value).Should().BeTrue();
        }

        [Theory]
        [InlineData("test-da7ab6b1-c513-581f-a1a0-7cdf17109deb", "da7ab6b1-c513-581f-a1a0-7cdf17109deb")]
        [InlineData("test-00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000")]
        public void WithValidValue(string value, string expectedGuidValue)
        {
            TestId testId = null;
            var expectedGuid = Guid.Parse(expectedGuidValue);

            Action action = () => testId = TestId.With(value);
            action.Should().NotThrow();

            testId.Should().NotBeNull();
            testId.Value.Should().Be(value);
            testId.GetGuid().Should().Be(expectedGuid);
        }

        [Fact]
        public void InputOutput()
        {
            var guid = Guid.NewGuid();
            
            var testId = TestId.With(guid);

            testId.GetGuid().Should().Be(guid);
        }

        [Fact]
        public void ShouldBeLowerCase()
        {
            var testId = TestId.New;

            testId.Value.Should().Be(testId.Value.ToLowerInvariant());
        }

        [Fact]
        public void New_IsValid()
        {
            var testId = TestId.New;

            TestId.IsValid(testId.Value).Should().BeTrue(testId.Value);
        }

        [Fact]
        public void NewComb_IsValid()
        {
            var testId = TestId.NewComb();

            TestId.IsValid(testId.Value).Should().BeTrue(testId.Value);
        }

        [Fact]
        public void NewDeterministic_IsValid()
        {
            var testId = TestId.NewDeterministic(Guid.NewGuid(), Guid.NewGuid().ToString());

            TestId.IsValid(testId.Value).Should().BeTrue(testId.Value);
        }

        [Theory]
        [InlineData("da7ab6b1-c513-581f-a1a0-7cdf17109deb")]
        [InlineData("thingyid-da7ab6b1-c513-581f-a1a0-7cdf17109deb")]
        [InlineData("thingy-769077C6-F84D-46E3-AD2E-828A576AAAF3")]
        [InlineData("thingy-pppppppp-pppp-pppp-pppp-pppppppppppp")]
        [InlineData("funny-da7ab6b1-c513-581f-a1a0-7cdf17109deb")]
        [InlineData(null)]
        [InlineData("")]
        public void CannotCreateBadIds(string badIdValue)
        {
            Action action = () => TestId.With(badIdValue);

            action.Should().Throw<ArgumentException>();
        }
    }
}
