
using System;
using System.Collections.Generic;
using Akkatecture.Commands;
using Akkatecture.Extensions;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class TestDistinctCommand : DistinctCommand<TestAggregate, TestAggregateId>
    {
        public int MagicNumber { get; }

        public TestDistinctCommand(
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
