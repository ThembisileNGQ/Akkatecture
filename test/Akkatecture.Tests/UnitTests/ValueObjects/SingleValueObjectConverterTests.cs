using System.ComponentModel;
using Akkatecture.TestHelpers;
using Akkatecture.ValueObjects;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Akkatecture.Tests.UnitTests.ValueObjects
{
    [Category(Categories.Unit)]
    public class SingleValueObjectConverterTests
    {
        [Theory]
        [InlineData("test  test", "\"test  test\"")]
        [InlineData("42", "\"42\"")]
        [InlineData("", "\"\"")]
        [InlineData(null, "null")]
        public void String_Serilization_ShouldBeExpectedJson(string value, string expectedJson)
        {
            var stringSvo = new StringSVO(value);
            
            var json = JsonConvert.SerializeObject(stringSvo);
            
            json.Should().Be(expectedJson);
        }

        [Fact]
        public void String_Deserialization_EmptyShouldResultInNull()
        {
            var stringSvo = JsonConvert.DeserializeObject<StringSVO>(string.Empty);
            
            stringSvo.Should().BeNull();
        }

        [Theory]
        [InlineData("\"\"", "")]
        [InlineData("\"test\"", "test")]
        public void String_Deserialization_ShouldBeExpectedValue(string json, string expectedValue)
        {
            var stringSvo = JsonConvert.DeserializeObject<StringSVO>(json);
            
            stringSvo.Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(0, "0")]
        [InlineData(42, "42")]
        [InlineData(-1, "-1")]
        public void Int_Serialization_ShouldBeExpectedJson(int value, string expectedJson)
        {
            var intSvo = new IntSVO(value);
            
            var json = JsonConvert.SerializeObject(intSvo);
            
            json.Should().Be(expectedJson);
        }

        [Theory]
        [InlineData("0", 0)]
        [InlineData("42", 42)]
        [InlineData("-1", -1)]
        public void Int_Deserialization_ShouldBeExpectedValue(string json, int expectedValue)
        {
            var intSvo = JsonConvert.DeserializeObject<IntSVO>(json);
            
            intSvo.Value.Should().Be(expectedValue);
        }

        [JsonConverter(typeof(SingleValueObjectConverter))]
        public class StringSVO : SingleValueObject<string>
        {
            public StringSVO(string value) : base(value) { }
        }

        [JsonConverter(typeof(SingleValueObjectConverter))]
        public class IntSVO : SingleValueObject<int>
        {
            public IntSVO(int value) : base(value) { }
        }
    }
}