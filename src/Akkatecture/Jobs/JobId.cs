using Akkatecture.Core;

namespace Akkatecture.Jobs
{
    public class JobId : Identity<JobId>, IJobId
    {
        public JobId(string value) 
            : base(value)
        {
        }
    }
}