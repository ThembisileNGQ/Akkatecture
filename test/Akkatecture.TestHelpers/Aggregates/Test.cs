using System;
using Akkatecture.Entities;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class Test : Entity<TestId>
    {
        public int TestsDone { get; }
        public bool DomainErrorEmitted { get; }

        public Test(
            TestId id,
            int testsDone,
            bool domainErrorEmitted)
            : base(id)
        {
            if (testsDone < 0)
                throw new ArgumentException(nameof(testsDone));

            TestsDone = testsDone;
            DomainErrorEmitted = domainErrorEmitted;
        }
    }
}