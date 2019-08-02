using Akkatecture.Core;
using Akkatecture.Jobs;

namespace Akkatecture.TestHelpers.Jobs
{
    public class TestJobId : Identity<TestJobId>, IJobId
    {
        public TestJobId(string value) 
            : base(value)
        {
        }
    }
}