using Akkatecture.Aggregates.ExecutionResults;

namespace Akkatecture.Jobs.Responses
{
    public class ScheduleAddedSuccess<TIdentity> : SuccessExecutionResult
        where TIdentity : IJobId
    {
        public TIdentity Id { get;  }

        public ScheduleAddedSuccess(TIdentity id)
        {
            Id = id;
        }
    }
}