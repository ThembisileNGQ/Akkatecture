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
        // ReSharper disable once InconsistentNaming
        private class StringSVO : SingleValueObject<string>
        {
            public StringSVO(string value) : base(value) { }
        }

        [JsonConverter(typeof(SingleValueObjectConverter))]
        // ReSharper disable once InconsistentNaming
        private class IntSVO : SingleValueObject<int>
        {
            public IntSVO(int value) : base(value) { }
        }
    }
}