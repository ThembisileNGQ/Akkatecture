using System;
using Akkatecture.Entities;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class Test : Entity<TestId>
    {
        public int TestsDone { get; private set; }
        public int DomainErrorsEmitted { get; private set; }


        public Test(TestId id)
            : this(id,0,0)
        {
            
        }
        
        public Test(
            TestId id,
            int testsDone,
            int domainErrorsEmitted)
            : base(id)
        {
            if (testsDone < 0)
                throw new ArgumentException(nameof(testsDone));
            if(domainErrorsEmitted < 0)
                throw new ArgumentException(nameof(domainErrorsEmitted));

            TestsDone = testsDone;
            DomainErrorsEmitted = domainErrorsEmitted;
        }

        public void SetTestsDone(int testsDone)
        {
            TestsDone = testsDone;
        }

        public void SetDomainErrorsEmitted(int domainErrorsEmitted)
        {
            DomainErrorsEmitted = domainErrorsEmitted;
        }
        
        
    }
}