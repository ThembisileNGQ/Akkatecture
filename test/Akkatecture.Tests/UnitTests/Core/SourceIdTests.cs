using System;
using Akkatecture.Core;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Core
{
    public class SourceIdTests
    {
        [Fact]
        public void InstantiatingSourceId_WithNullString_ThrowsException()
        {
            this.Invoking(test => new SourceId(null))
                .Should().Throw<ArgumentNullException>();
        }
    }
}