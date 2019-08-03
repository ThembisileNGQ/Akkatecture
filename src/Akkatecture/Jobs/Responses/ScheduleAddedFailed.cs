using System.Collections.Generic;
using Akkatecture.Aggregates.ExecutionResults;

namespace Akkatecture.Jobs.Responses
{
    public class ScheduleAddedFailed<TIdentity> : FailedExecutionResult
        where TIdentity : IJobId
    {
        public ScheduleAddedFailed(IEnumerable<string> errors) : base(errors)
        {
        }
    }
}