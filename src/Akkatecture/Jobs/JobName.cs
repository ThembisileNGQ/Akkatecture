using Akkatecture.ValueObjects;

namespace Akkatecture.Jobs
{
    public class JobName: SingleValueObject<string>, IJobName
    {
        public JobName(string value) 
            : base(value)
        {
        }
    }
}