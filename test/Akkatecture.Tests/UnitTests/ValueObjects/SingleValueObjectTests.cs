// The MIT License (MIT)
//
// Copyright (c) 2015-2020 Rasmus Mikkelsen
// Copyright (c) 2015-2020 eBay Software Foundation
// Modified from original source https://github.com/eventflow/EventFlow
//
// Copyright (c) 2018 - 2020 Lutando Ngqakaza
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
using System.Collections.Generic;
using System.Linq;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.ValueObjects
{
    public class SingleValueObjectTests
    {
         public class StringSingleValue : SingleValueObject<string>
        {
            public StringSingleValue(string value) 
                : base(value) { }
        }

        public enum MagicEnum
        {
            Two = 2,
            Zero = 0,
            Three = 3,
            One = 1
        }

        public class MagicEnumSingleValue : SingleValueObject<MagicEnum>
        {
            public MagicEnumSingleValue(MagicEnum value) 
                : base(value) { }
        }

        [Fact]
        public void SVOWithEnumList_IsEquivalent_ExactlyOrderedEnumList()
        {
            var values = Enumerable.Range(0,10).Select(x => TestId.New.Value).ToList();
            var orderedValues = values.OrderBy(s => s).ToList();
            values.Should().NotEqual(orderedValues); // Data test
            var singleValueObjects = values.Select(s => new StringSingleValue(s)).ToList();

            var orderedSingleValueObjects = singleValueObjects.OrderBy(v => v).ToList();

            orderedSingleValueObjects.Select(v => v.Value).Should().BeEquivalentTo(
                orderedValues,
                o => o.WithStrictOrdering());
        }

        [Fact]
        public void SVOWithEnumList_IsEquivalent_AlternativelyOrderedEnumList()
        {
            var values = new List<MagicEnum>
            {
                MagicEnum.One,
                MagicEnum.Zero,
                MagicEnum.Three
            };
            var orderedValues = values.OrderBy(s => s).ToList();
            values.Should().NotEqual(orderedValues); 
            var singleValueObjects = values.Select(s => new MagicEnumSingleValue(s)).ToList();

            var orderedSingleValueObjects = singleValueObjects.OrderBy(v => v).ToList();

            orderedSingleValueObjects.Select(v => v.Value).Should().BeEquivalentTo(
                orderedValues,
                o => o.WithStrictOrdering());
        }

        [Fact]
        public void SVOWithEnum_WhenInstantiated_ProtectsAgainsInvalidEnumValues()
        {
            this.Invoking(test => new MagicEnumSingleValue((MagicEnum) 42))
                .Should().Throw<ArgumentException>().And.Message.Should()
                .Be("The value '42' isn't defined in enum 'MagicEnum'");
        }

        [Fact]
        public void SVOList_WhenComparedWithStriclyOrderedList_IsEquivalent()
        {
            var values = new[]
                {
                    new MagicEnumSingleValue(MagicEnum.Zero), 
                    new MagicEnumSingleValue(MagicEnum.Three), 
                    new MagicEnumSingleValue(MagicEnum.One), 
                    new MagicEnumSingleValue(MagicEnum.Two), 
                };
            
            var orderedValues = values
                .OrderBy(v => v)
                .Select(v => v.Value)
                .ToList();
            
            orderedValues.Should().BeEquivalentTo(
                new []
                {
                    MagicEnum.Zero,
                    MagicEnum.One,
                    MagicEnum.Two,
                    MagicEnum.Three,
                },
                o => o.WithStrictOrdering());
        }

        [Fact]
        public void SVO_WhenComparedWithNull_DoesNotEquals()
        {
            var svo = new StringSingleValue(TestId.New.Value);
            var null_ = null as StringSingleValue;

            // ReSharper disable once ExpressionIsAlwaysNull
            svo.Equals(null_).Should().BeFalse();
        }

        [Fact]
        public void SVO_WhenComparedWithDifferentOnes_EqualsForSameValues()
        {
            var value = TestId.New.Value;
            var svo1 = new StringSingleValue(value);
            var svo2 = new StringSingleValue(value);

            (svo1 == svo2).Should().BeTrue();
            svo1.Equals(svo2).Should().BeTrue();
        }

        [Fact]
        public void SVO_WhenComparedWithDifferentOnes_EqualsForDifferentValues()
        {
            var value1 = TestId.New.Value;
            var value2 = TestId.New.Value;
            var svo1 = new StringSingleValue(value1);
            var svo2 = new StringSingleValue(value2);

            (svo1 == svo2).Should().BeFalse();
            svo1.Equals(svo2).Should().BeFalse();
        }
        
        [Fact]
        public void SVO_WhenComparedWithNull_ThrowsException()
        {
            var value = TestId.New.Value;
            var svo = new StringSingleValue(value);

            this.Invoking(test => svo.CompareTo(null)).Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void SVO_WhenComparedWithOtherType_ThrowsException()
        {
            var value = TestId.New.Value;
            var svo = new StringSingleValue(value);
            var invalidSvo = new MagicEnumSingleValue(MagicEnum.Two);

            this.Invoking(test => svo.CompareTo(invalidSvo)).Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void SVO_WhenComparedSameValue_ThrowsException()
        {
            var value = TestId.New.Value;
            var svo1 = new StringSingleValue(value);
            var svo2 = new StringSingleValue(value);

            svo1.CompareTo(svo2).Should().Be(0);
        }
    }
}