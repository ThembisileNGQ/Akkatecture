// The MIT License (MIT)
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

using System.Collections.Generic;
using System.Linq;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Snapshots;

namespace Akkatecture.TestHelpers.Aggregates
{

    public class TestAggregateState : AggregateState<TestAggregate, TestAggregateId>,
        IApply<TestAddedEvent>,
        IApply<TestReceivedEvent>,
        IApply<TestSentEvent>,
        IApply<TestCreatedEvent>,
        IHydrate<TestAggregateSnapshot>
    {
        public List<Test> TestCollection { get; private set; }
        public bool FromHydration { get; private set; }

        public void Apply(TestCreatedEvent aggregateEvent)
        {
            TestCollection = new List<Test>();
            FromHydration = false;
        }
        
        public void Apply(TestAddedEvent aggregateEvent)
        {
            TestCollection.Add(aggregateEvent.Test);
        }

        public void Apply(TestReceivedEvent aggregateEvent)
        {
            TestCollection.Add(aggregateEvent.Test);
        }

        public void Apply(TestSentEvent aggregateEvent)
        {
            TestCollection.RemoveAll(x => x.Id == aggregateEvent.Test.Id);
        }
        public void Hydrate(TestAggregateSnapshot aggregateSnapshot)
        {
            TestCollection = aggregateSnapshot.Tests.Select(x => new Test(TestId.With(x.Id))).ToList();
            FromHydration = true;
        }
    }

}