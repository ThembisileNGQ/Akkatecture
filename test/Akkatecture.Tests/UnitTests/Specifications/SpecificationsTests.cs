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