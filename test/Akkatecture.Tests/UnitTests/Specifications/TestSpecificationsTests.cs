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