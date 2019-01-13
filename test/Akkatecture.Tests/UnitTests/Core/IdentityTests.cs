// The MIT License (MIT)
//
// Copyright (c) 2015-2019 Rasmus Mikkelsen
// Copyright (c) 2015-2019 eBay Software Foundation
// Modified from original source https://github.com/eventflow/EventFlow
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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

            var testId = TestAggregateId.NewDeterministic(namespaceId, name);

            testId.Value.Should().Be("testaggregate-da7ab6b1-c513-581f-a1a0-7cdf17109deb");
            TestAggregateId.IsValid(testId.Value).Should().BeTrue();
        }

        [Theory]
        [InlineData("testaggregate-da7ab6b1-c513-581f-a1a0-7cdf17109deb", "da7ab6b1-c513-581f-a1a0-7cdf17109deb")]
        [InlineData("testaggregate-00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000")]
        public void Identity_Valid_WithValue(string value, string expectedGuidValue)
        {
            TestAggregateId testId = null;
            var expectedGuid = Guid.Parse(expectedGuidValue);

            Action action = () => testId = TestAggregateId.With(value);
            action.Should().NotThrow();

            testId.Should().NotBeNull();
            testId.Value.Should().Be(value);
            testId.GetGuid().Should().Be(expectedGuid);
        }

        [Fact]
        public void Identity_Created_WithGuid()
        {
            var guid = Guid.NewGuid();
            
            var testId = TestAggregateId.With(guid);

            testId.GetGuid().Should().Be(guid);
        }

        [Fact]
        public void Identity_Value_ShouldBeLowerCase()
        {
            var testId = TestAggregateId.New;

            testId.Value.Should().Be(testId.Value.ToLowerInvariant());
        }

        [Fact]
        public void Identity_New_IsValid()
        {
            var testId = TestAggregateId.New;

            TestAggregateId.IsValid(testId.Value).Should().BeTrue(testId.Value);
        }

        [Fact]
        public void Identity_NewComb_IsValid()
        {
            var testId = TestAggregateId.NewComb();

            TestAggregateId.IsValid(testId.Value).Should().BeTrue(testId.Value);
        }

        [Fact]
        public void Identity_NewDeterministic_IsValid()
        {
            var testId = TestAggregateId.NewDeterministic(Guid.NewGuid(), Guid.NewGuid().ToString());

            TestAggregateId.IsValid(testId.Value).Should().BeTrue(testId.Value);
        }

        [Theory]
        [InlineData("da7ab6b1-c513-581f-a1a0-7cdf17109deb")]
        [InlineData("thingyid-da7ab6b1-c513-581f-a1a0-7cdf17109deb")]
        [InlineData("thingy-769077C6-F84D-46E3-AD2E-828A576AAAF3")]
        [InlineData("thingy-pppppppp-pppp-pppp-pppp-pppppppppppp")]
        [InlineData("funny-da7ab6b1-c513-581f-a1a0-7cdf17109deb")]
        [InlineData(null)]
        [InlineData("")]
        public void Identity_CannotCreate_WithBadIds(string badIdValue)
        {
            Action action = () => TestAggregateId.With(badIdValue);

            action.Should().Throw<ArgumentException>();
        }
    }
}
