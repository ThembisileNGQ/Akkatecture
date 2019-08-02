using Akkatecture.Core;

namespace Akkatecture.ScheduledJobs
{
    public class JobId : Identity<JobId>, IJobId
    {
        public JobId(string value) 
            : base(value)
        {
        }
    }
}