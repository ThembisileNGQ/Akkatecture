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
using Akkatecture.Commands;
using Akkatecture.Extensions;
using Akkatecture.TestHelpers.Aggregates;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Commands
{
    public class DistinctCommandTests
    {
        [Theory]
        [InlineData("testaggregate-4b1e7b48-18f1-4215-91d9-903cffdab3d8", 1, "command-c3918596-b704-5e6a-9bfc-9876b359f0f0")]
        [InlineData("testaggregate-4b1e7b48-18f1-4215-91d9-903cffdab3d8", 2, "command-2fc95416-f301-581d-8173-6624677ac172")]
        [InlineData("testaggregate-6a2a04bd-bbc8-44ac-80ac-b0ca56897bc0", 2, "command-a123dc1f-781b-558c-8740-4fb4d2f3b018")]
        public void Arguments(string aggregateId, int magicNumber, string expectedSouceId)
        {
            var testId = TestAggregateId.With(aggregateId);
            var command = new MyDistinctCommand(testId, magicNumber);

            var sourceId = command.SourceId;
            
            sourceId.Value.Should().Be(expectedSouceId);
        }

        public class MyDistinctCommand : DistinctCommand<TestAggregate, TestAggregateId>
        {
            public int MagicNumber { get; }

            public MyDistinctCommand(
                TestAggregateId aggregateId,
                int magicNumber) 
                : base(aggregateId)
            {
                MagicNumber = magicNumber;
            }

            protected override IEnumerable<byte[]> GetSourceIdComponents()
            {
                yield return BitConverter.GetBytes(MagicNumber);
                yield return AggregateId.GetBytes();
            }
        }
    }
}