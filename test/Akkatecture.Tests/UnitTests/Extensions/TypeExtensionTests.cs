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
using System.Collections.Generic;
using System.ComponentModel;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Extensions;
using Akkatecture.TestHelpers;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Extensions
{
    [Category(Categories.Unit)]
    public class TypeExtensionTests
    {
        [Theory]
        [InlineData(typeof(string), "String")]
        [InlineData(typeof(int), "Int32")]
        [InlineData(typeof(IEnumerable<>), "IEnumerable<>")]
        [InlineData(typeof(KeyValuePair<,>), "KeyValuePair<,>")]
        [InlineData(typeof(IEnumerable<string>), "IEnumerable<String>")]
        [InlineData(typeof(IEnumerable<IEnumerable<string>>), "IEnumerable<IEnumerable<String>>")]
        [InlineData(typeof(KeyValuePair<bool, long>), "KeyValuePair<Boolean,Int64>")]
        [InlineData(typeof(KeyValuePair<KeyValuePair<bool, long>, KeyValuePair<bool, long>>), "KeyValuePair<KeyValuePair<Boolean,Int64>,KeyValuePair<Boolean,Int64>>")]
        public void PrettyPrint_Output_ShouldBeExpected(Type type, string expectedPrettyPrint)
        {
            var prettyPrint = type.PrettyPrint();
            
            prettyPrint.Should().Be(expectedPrettyPrint);
        }

        [Theory]
        [InlineData(typeof(FooAggregateWithOutAttribute), "FooAggregateWithOutAttribute")]
        [InlineData(typeof(FooAggregateWithAttribute), "BetterNameForAggregate")]
        public void AggregateName_FromType_ShouldBeExpected(Type aggregateType, string expectedAggregateName)
        {
            var aggregateName = aggregateType.GetAggregateName();
            
            aggregateName.Value.Should().Be(expectedAggregateName);
        }

        public class FooId : Identity<FooId>
        {
            public FooId(string value) : base(value)
            {
            }
        }

        public class FooAggregateWithOutAttribute : AggregateRoot<FooAggregateWithOutAttribute, FooId, FooStateWithOutAttribute>
        {
            public FooAggregateWithOutAttribute(FooId id) : base(id)
            {
            }
        }

        [AggregateName("BetterNameForAggregate")]
        public class FooAggregateWithAttribute : AggregateRoot<FooAggregateWithAttribute, FooId, FooStateWithAttribute>
        {
            public FooAggregateWithAttribute(FooId id) : base(id)
            {
            }
        }

        public class FooStateWithAttribute : AggregateState<FooAggregateWithAttribute, FooId,
            IMessageApplier<FooAggregateWithAttribute, FooId>>
        {
                
        }

        public class FooStateWithOutAttribute : AggregateState<FooAggregateWithOutAttribute, FooId,
            IMessageApplier<FooAggregateWithOutAttribute, FooId>>
        {

        }
    }
}
