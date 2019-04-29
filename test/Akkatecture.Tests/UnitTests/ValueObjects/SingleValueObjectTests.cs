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
        public void Ordering()
        {
            // Arrange
            var values = Enumerable.Range(0,10).Select(x => TestId.New.Value).ToList();
            var orderedValues = values.OrderBy(s => s).ToList();
            values.Should().NotEqual(orderedValues); // Data test
            var singleValueObjects = values.Select(s => new StringSingleValue(s)).ToList();

            // Act
            var orderedSingleValueObjects = singleValueObjects.OrderBy(v => v).ToList();

            // Assert
            orderedSingleValueObjects.Select(v => v.Value).Should().BeEquivalentTo(
                orderedValues,
                o => o.WithStrictOrdering());
        }

        [Fact]
        public void EnumOrdering()
        {
            // Arrange
            var values = new List<MagicEnum>
            {
                MagicEnum.One,
                MagicEnum.Zero,
                MagicEnum.Three
            };
            var orderedValues = values.OrderBy(s => s).ToList();
            values.Should().NotEqual(orderedValues); 
            var singleValueObjects = values.Select(s => new MagicEnumSingleValue(s)).ToList();

            // Act
            var orderedSingleValueObjects = singleValueObjects.OrderBy(v => v).ToList();

            // Assert
            orderedSingleValueObjects.Select(v => v.Value).Should().BeEquivalentTo(
                orderedValues,
                o => o.WithStrictOrdering());
        }

        [Fact]
        public void ProtectAgainsInvalidEnumValues()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentException>(() => new MagicEnumSingleValue((MagicEnum)42));
            exception.Message.Should().Be("The value '42' isn't defined in enum 'MagicEnum'");
        }

        [Fact]
        public void EnumOrderingManual()
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
        public void NullEquals()
        {
            // Arrange
            var obj = new StringSingleValue(TestId.New.Value);
            var null_ = null as StringSingleValue;

            // Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            obj.Equals(null_).Should().BeFalse();
        }

        [Fact]
        public void EqualsForSameValues()
        {
            // Arrange
            var value = TestId.New.Value;
            var obj1 = new StringSingleValue(value);
            var obj2 = new StringSingleValue(value);

            // Assert
            (obj1 == obj2).Should().BeTrue();
            obj1.Equals(obj2).Should().BeTrue();
        }

        [Fact]
        public void EqualsForDifferentValues()
        {
            // Arrange
            var value1 = TestId.New.Value;
            var value2 = TestId.New.Value;
            var obj1 = new StringSingleValue(value1);
            var obj2 = new StringSingleValue(value2);

            // Assert
            (obj1 == obj2).Should().BeFalse();
            obj1.Equals(obj2).Should().BeFalse();
        }
    }
}