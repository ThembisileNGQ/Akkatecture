using System;
using Akkatecture.Entities;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class Test : Entity<TestId>
    {
        public int TestsDone { get; private set; }
        
        public Test(TestId id)
            : this(id,0)
        {
            
        }
        
        public Test(
            TestId id,
            int testsDone)
            : base(id)
        {
            if (testsDone < 0)
                throw new ArgumentException(nameof(testsDone));

            TestsDone = testsDone;
        }

        public void SetTestsDone(int testsDone)
        {
            TestsDone = testsDone;
        }
 
    }
}