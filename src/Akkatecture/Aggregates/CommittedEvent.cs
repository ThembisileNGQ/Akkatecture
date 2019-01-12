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
using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public class CommittedEvent<TAggregate, TIdentity, TAggregateEvent> : ICommittedEvent<TAggregate, TIdentity, TAggregateEvent>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
    {
        public TIdentity AggregateIdentity { get; }
        public TAggregateEvent AggregateEvent { get; }
        public Metadata Metadata { get; }
        public long AggregateSequenceNumber { get; }
        public DateTimeOffset Timestamp { get; }

        public CommittedEvent(
            TIdentity aggregateIdentity,
            TAggregateEvent aggregateEvent,
            Metadata metadata,
            DateTimeOffset timestamp,
            long aggregateSequenceNumber)
        {
            if (aggregateEvent == null) throw new ArgumentNullException(nameof(aggregateEvent));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            if (timestamp == default(DateTimeOffset)) throw new ArgumentNullException(nameof(timestamp));
            if (aggregateEvent == null) throw new ArgumentNullException(nameof(aggregateEvent));
            if (aggregateIdentity == null || string.IsNullOrEmpty(aggregateIdentity.Value)) throw new ArgumentNullException(nameof(aggregateIdentity));
            
            
            AggregateIdentity = aggregateIdentity;
            AggregateSequenceNumber = aggregateSequenceNumber;
            AggregateIdentity = aggregateIdentity;
            AggregateEvent = aggregateEvent;
            Metadata = metadata;
            Timestamp = timestamp;
        }
        
        public IAggregateEvent GetAggregateEvent()
        {
            return AggregateEvent;
        }

        
    }
}