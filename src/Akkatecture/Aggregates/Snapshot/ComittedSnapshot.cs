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

using System;
using Akkatecture.Core;

namespace Akkatecture.Aggregates.Snapshot
{
    public class ComittedSnapshot<TAggregate, TIdentity, TAggregateSnapshot>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TAggregateSnapshot : IAggregateSnapshot<TAggregate, TIdentity>
    {
        public TIdentity AggregateIdentity { get; }
        public TAggregateSnapshot AggregateSnapshot { get; }
        public SnapshotMetadata Metadata { get; }
        public long AggregateSequenceNumber { get; }
        public DateTimeOffset Timestamp { get; }

        public ComittedSnapshot(
            TIdentity aggregateIdentity,
            TAggregateSnapshot aggregateSnapshot,
            SnapshotMetadata metadata,
            DateTimeOffset timestamp,
            long aggregateSequenceNumber)
        {
            if (aggregateSnapshot == null) throw new ArgumentNullException(nameof(aggregateSnapshot));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            if (timestamp == default(DateTimeOffset)) throw new ArgumentNullException(nameof(timestamp));
            if (aggregateSnapshot == null) throw new ArgumentNullException(nameof(aggregateSnapshot));
            if (aggregateIdentity == null || string.IsNullOrEmpty(aggregateIdentity.Value)) throw new ArgumentNullException(nameof(aggregateIdentity));


            AggregateIdentity = aggregateIdentity;
            AggregateSequenceNumber = aggregateSequenceNumber;
            AggregateIdentity = aggregateIdentity;
            AggregateSnapshot = aggregateSnapshot;
            Metadata = metadata;
            Timestamp = timestamp;
        }
    }
}
